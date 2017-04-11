import Backbone from 'backbone';
import React from 'react';
import ReactDOMServer from 'react-dom/server';
import 'react.backbone';
import localStorage from 'localforage';
import Promise from 'bluebird';
import mapboxgl from 'mapbox-gl/dist/mapbox-gl-dev';
import turfCentroid from '@turf/centroid';
import turfEnvelope from '@turf/envelope';
import $ from 'jquery';
import classNames from 'classnames';
import {
	groupBy,
	map,
	each,
	find,
	filter,
	indexOf,
	xor,
	extend,
	startsWith,
	keys,
	head,
	last,
	concat,
	clone,
	get,
	set,
	trim,
	isFunction,
	includes
} from 'lodash';

import 'select2';

import GTUCollection from 'collections/gtu';
import DMap from 'models/print/dmap';
import GtuCollection from 'collections/gtu';
import UserCollection from 'collections/user';
import UserModel from 'models/user';
import TaskModel from 'models/task';
import GeoCollection from 'collections/geo';

import BaseView from 'views/base';
import AssignView from 'views/gtu/assign';
import EmployeeView from 'views/user/employee';
import EditView from 'views/monitor/edit';

const TAG = '[CAMPAIGN-MONITOR]';
export default React.createBackboneClass({
	mixins: [BaseView],
	getInitialState: function () {
		return {
			inputText: '',
			placeHolder: 'Please input Address or Zip code here',
			activeItem: null,
			searchTimeout: null
		}
	},
	search: function () {
		clearTimeout(this.state.searchTimeout);
		var self = this;
		this.state.searchTimeout = setTimeout(() => {
			var geocode = trim(self.state.inputText);
			if (geocode.length > 0) {
				var promiseQuery = null;
				if (/^\d+$/.test(geocode)) {
					promiseQuery = self.getCollection().fetchByZipcode(geocode);
				} else {
					promiseQuery = self.getCollection().fetchByAddress(geocode);
				}
				promiseQuery.then(geoJson => {
					self.publish('Campaign.Monitor.GeoResult', geoJson);
				});
			}
		}, 500);
	},
	onInputChange: function (event) {
		var self = this;
		this.setState({
			inputText: event.target.value
		}, () => {
			self.search();
		})
	},
	onInputKeyUp: function (event) {
		var results = this.getCollection() || [];
		if (results.length == 0) {
			return;
		}
		switch (event.key) {
		case 'ArrowUp':
			var index = results.indexOf(this.state.activeItem);
			index--;
			index = Math.max(0, index);
			this.onSelectItem(results.at(index));
			event.preventDefault();
			event.stopPropagation();
			break;
		case 'ArrowDown':
			var index = results.indexOf(this.state.activeItem);
			index++;
			index = Math.min(index, results.length - 1);
			this.onSelectItem(results.at(index));
			event.preventDefault();
			event.stopPropagation();
			break;
		case 'Enter':
			var activeItem = null;
			if (this.state.activeItem == null) {
				activeItem = results.at(0);
			} else {
				activeItem = this.state.activeItem;
			}
			this.onSelectItem(activeItem);
			event.preventDefault();
			event.stopPropagation();
			break;
		}
	},
	onSelectItem: function (item, callback) {
		if (item) {
			var self = this;
			this.setState({
				activeItem: item,
				inputText: item.get('place')
			}, () => {
				this.publish('Campaign.Monitor.FlyToLocation', item.get('latlng'));
				if (isFunction(callback)) {
					callback.call(self);
				}
			});
		}
	},
	clearUp: function () {
		var self = this;
		var placeHolder = '';
		if (this.state.activeItem) {
			placeHolder = this.state.activeItem.get('text');
		}
		this.setState({
			activeItem: null,
			inputText: '',
			placeHolder: placeHolder
		}, () => {
			self.getCollection().reset();
			$(window).focus();
			self.publish('Campaign.Monitor.GeoResult', {
				type: 'FeatureCollection',
				features: []
			});
		});
	},
	stopClearUp: function (e) {
		e.preventDefault();
		e.stopPropagation();
	},
	render: function () {
		var self = this;
		var results = this.getCollection() || [];
		var geoClass = classNames("geocode", {
			active: results.length > 1
		});
		return (
			<div className={geoClass} onKeyUp={this.onInputKeyUp} onClick={this.stopClearUp}>
				<input type="text" value={this.state.inputText} onKeyUp={this.onInputKeyUp} onChange={this.onInputChange} placeholder={this.state.placeHolder} />
				<ul className="accordion">
					{results.map((item, index)=>{
						let itemClass = classNames('accordion-item', {
							active: self.state.activeItem == item
						});
						return (
							<li key={item.get('id')} className={itemClass} onClick={self.onSelectItem.bind(self, item)}>
								<i className="fa fa-stop"></i>
								<span>{index + 1}</span>
								&nbsp;{item.get('place')}
							</li>
						);
					})}
				</ul>
			</div>
		);
	},
	componentWillUnmount: function () {
		$(window).off('click.geo');
	},
	componentDidMount: function () {
		var self = this;
		$(window).on('click.geo', () => {
			self.clearUp();
		});
	}
});