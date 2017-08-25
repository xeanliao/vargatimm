import Backbone from 'backbone';
import Promise from 'bluebird';
import {
	each
} from 'lodash';

import Model from 'models/geo';

export default Backbone.Collection.extend({
	model: Model,
	fetchByZipcode: function (zipcode) {
		var collection = this;
		return new Promise((resolve, reject) => {
			$.ajax({
				url: `//api.mapbox.com/geocoding/v5/mapbox.places/${zipcode}.json`,
				method: 'GET',
				data: {
					country: 'us',
					types: 'postcode',
					access_token: MapboxToken
				}
			}).then(result => {
				var models = [];
				var geoJson = {
					type: 'FeatureCollection',
					features: []
				};
				each(result.features, (item, index) => {
					models.push({
						id: item.id,
						place: item.place_name,
						text: item.text,
						latlng: item.geometry.coordinates
					});
					geoJson.features.push({
						type: "Feature",
						geometry: item.geometry,
						properties: {
							id: item.id,
							text: item.text,
							serial: index + 1
						}
					})
				});
				collection.set(models);
				resolve(geoJson);
			});
		});
	},
	fetchByAddress: function (address) {
		var collection = this;
		return new Promise((resolve, reject) => {
			$.ajax({
				url: `//api.mapbox.com/geocoding/v5/mapbox.places/${address}.json`,
				method: 'GET',
				data: {
					country: 'us',
					access_token: MapboxToken
				}
			}).then(result => {
				var models = [];
				var geoJson = {
					type: 'FeatureCollection',
					features: []
				};

				each(result.features, (item, index) => {
					models.push({
						id: item.id,
						place: item.place_name,
						text: item.text,
						latlng: item.geometry.coordinates
					});
					geoJson.features.push({
						type: "Feature",
						geometry: item.geometry,
						properties: {
							id: item.id,
							text: item.text,
							serial: index + 1
						}
					})
				});
				collection.set(models);
				resolve(geoJson);
			});
		});
	}
});