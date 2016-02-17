define([
	'underscore',
	'moment',
	'backbone',
	'react',
	'pubsub',
	'views/distribution/publish',
	'views/distribution/dismiss',
	'react.backbone'
], function (_, moment, Backbone, React, Topic, PublishView, DismissView) {
	var actionHandler = null,
		searchHandle = null;
	var DistributionRow = React.createBackboneClass({
		getDefaultProps: function(){
			return {
				address: null,
				icon: null,
				name: null
			};
		},
		componentDidMount: function(){
			$('.has-tip').foundation();
		},
		componentDidUpdate: function(){
		},
		onDismiss: function(e){
			e.preventDefault();
			e.stopPropagation();
			var model = this.getModel();
			var confirmResult = confirm('Are you sure you would like to move \r\n' + model.getDisplayName() + '\r\nto Campaigns? Any changes that were made will be lost.');
			if(confirmResult){
				Topic.publish('showDialog', DismissView);
				actionHandler && Topic.unsubscribe(actionHandler);
				actionHandler = Topic.subscribe('distribution/dismiss', function(user){
					model.dismissToCampaign(user, {
						success: function(result){
							Topic.publish('showDialog');
							if(result && result.success){
								Topic.publish('distribution/refresh');
							}else{
								alert("something wrong");
							}
						}
					});
				});
			}
		},
		onPublishToMonitors: function(e){
			e.preventDefault();
			e.stopPropagation();
			var model = this.getModel();
			Topic.publish('showDialog', PublishView);
			actionHandler && Topic.unsubscribe(actionHandler);
			actionHandler = Topic.subscribe('distribution/publish', function(user){
				model.publishToMonitor(user, {
					success: function(result){
						Topic.publish('showDialog');
						if(result && result.success){
							Topic.publish('distribution/refresh');	
						}else{
							alert(result.error);
						}
					}
				});
			});
		},
		onGotoDMap: function(id){
			window.location.hash = 'frame/DistributionMap.aspx?cid=' + id;
		},
		render: function(){
			var model = this.getModel();
			var date = model.get('Date');
			var displayDate = date ? moment(date).format("MMM DD, YYYY") : '';
			return (
				<div className="row scroll-list-item" onDoubleClick={this.onGotoDMap.bind(null, model.get('Id'))}>
					<div className="hide-for-small-only medium-2 columns">
						{model.get('ClientName')}
					</div>
					<div className="small-12 medium-5 columns">
						{model.get('ClientCode')}
					</div>
					<div className="hide-for-small-only medium-2 columns">
						{displayDate}
					</div>
					<div className="small-12 medium-3 columns">
						<span className="show-for-large">{model.get('AreaDescription')}</span>
						<div className="float-right tool-bar">
							<button onClick={this.onPublishToMonitors} className="button">
								<i className="fa fa-upload"></i><small>Publish</small>
							</button>
							<button onClick={this.onDismiss} className="button has-tip top" title="dismiss" 
								data-tooltip aria-haspopup="true" 
								data-disable-hover='false' tabIndex="1">
								<i className="fa fa-reply"></i>
							</button>
						</div>
					</div>
				</div>
			);
		}
	});
	var DistributionList = React.createBackboneClass({
		getInitialState: function(){
			return {
				orderByFiled: null,
				orderByAsc: false,
				search: null,
				filterField: null,
				filterValues: []
			};
		},
		componentDidMount: function(){
			var self = this;
			Topic.subscribe('distribution/refresh', function(){
				self.getCollection().fetchForDistribution();
			});
			searchHandle && Topic.unsubscribe(searchHandle);
			searchHandle = Topic.subscribe('search', function(words){
				console.log('on search');
				self.setState({
					search: words,
					filterField: null,
					filterValues: []
				});
			});

			$("#distribution-filter-ddl-ClientName, #distribution-filter-ddl-ClientCode, #distribution-filter-ddl-Date, #distribution-filter-ddl-AreaDescription").foundation();
		},
		componentWillUnmount: function(){
			searchHandle && Topic.unsubscribe(searchHandle);
			actionHandler && Topic.unsubscribe(actionHandler);
		},
		onOrderBy: function(field, reactObj, reactId, e){
			e.preventDefault();
			e.stopPropagation();
			if(this.state.orderByFiled == field){
				this.setState({orderByAsc: !this.state.orderByAsc});
			}else{
				this.setState({
					orderByFiled: field,
					orderByAsc: true
				});
			}
		},
		onFilter: function(field, e){
			e.preventDefault();
			e.stopPropagation();
			var els = $(":checked", e.currentTarget),
				values = _.map(els, function(item){
					return $(item).val();
				});

			this.setState({
				filterField: values.length > 0 ? field : null,
				filterValues: values.length > 0 ? values : [],
				search: null
			});
			$('#distribution-filter-ddl-' + field).foundation('close');
		},
		onClearFilter: function(field, e){
			e.preventDefault();
			e.stopPropagation();
			this.setState({
				filterField: null,
				filterValues: [],
				search: null
			});
			$('#distribution-filter-ddl-' + field).foundation('close');
			$(e.currentTarget).closest('form').get(0).reset();
		},
		renderHeader: function(field, displayName){
			var dataSource = this.getCollection(),
				sortIcon = null,
				filterIcon = null,
				filterMenu = null;
			dataSource = dataSource ? dataSource.toArray() : [];

			if(this.state.orderByFiled == field){
				if(this.state.orderByAsc){
					sortIcon = <i className="fa fa-sort-up active"></i>
				}else{
					sortIcon = <i className="fa fa-sort-down active"></i>
				}
			}else{
				sortIcon = <i className="fa fa-sort"></i>
			}
			sortIcon = <a onClick={this.onOrderBy.bind(null, field)}>&nbsp;{sortIcon}</a>

			if(this.state.filterField && this.state.filterField == field){
				filterIcon = (
					<a data-toggle={"distribution-filter-ddl-" + field}>
						&nbsp;<i className="fa fa-filter active"></i>
					</a>
				);
			}
			if(dataSource){
				var fieldValues = _.map(dataSource, function(i){
					var fieldValue = i.get(field);
					var dateCheck = moment(fieldValue, moment.ISO_8601);
					if(dateCheck.isValid()){
						return dateCheck.format("MMM DD, YYYY");
					}
					return fieldValue;
				});
				var menuItems = _.uniq(fieldValues).sort();
				filterMenu = (
					<div id={"distribution-filter-ddl-" + field} 
						className="dropdown-pane bottom" 
						style={{width: 'auto'}}
						data-dropdown 
						data-close-on-click="true" 
						data-auto-focus="false">
						<form onSubmit={this.onFilter.bind(this, field)}>
							<ul className="vertical menu">
								{menuItems.map(function(item, index){
									return (
										<li key={field + index}><input id={field + index} name={field} type="checkbox" value={item} /><label htmlFor={field + index}>{item}</label></li>
									)
								})}
							</ul>
							<button className="button tiny success" type="submit">Filter</button>
							<button className="button tiny warning" type="reset" onClick={this.onClearFilter.bind(this, field)}>Clear</button>
						</form>
					</div>
				);
			}
			
			return(
				<div>
					<a data-toggle={"distribution-filter-ddl-" + field}>
						{displayName}
					</a>
					{sortIcon}
					{filterIcon}
					{filterMenu}
				</div>
			);
		},
		getDataSource: function(){
			var dataSource = this.getCollection();
			dataSource = dataSource ? dataSource.toArray() : [];
			if(this.state.orderByFiled){
				dataSource = _.orderBy(dataSource, ['attributes.' + this.state.orderByFiled], [this.state.orderByAsc ? 'asc' : 'desc']);
			}
			if(this.state.search){
				var keyword = this.state.search.toLowerCase(),
					values = null;
				dataSource = _.filter(dataSource, function(i) {
					values  = _.values(i.attributes);
					return _.some(values, function(i){
						var dateCheck = moment(i, moment.ISO_8601);
						if(dateCheck.isValid()){
							return dateCheck.format("MMM DD YYYY MMM DD, YYYY YYYY-MM-DD MM/DD/YYYY YYYY MM MMM DD").toLowerCase().indexOf(keyword) > -1;
						}
						return _.toString(i).toLowerCase().indexOf(keyword) > -1;
					});
				});
			}else if(this.state.filterField && this.state.filterValues){
				var filterField = this.state.filterField,
					filterValues = this.state.filterValues;
				dataSource = _.filter(dataSource, function(i) {
					var fieldValue = i.get(filterField);
					var dateCheck = moment(fieldValue, moment.ISO_8601);
					if(dateCheck.isValid()){
						return _.indexOf(filterValues, dateCheck.format("MMM DD, YYYY")) > -1;
					}
					return _.indexOf(filterValues, fieldValue) > -1;
				});
			}
			return dataSource;
		},
		render: function () {
			var list = this.getDataSource();
			return ( 
				<div className="section row">
					<div className="small-12 columns">
						<div className="section-header">
							<div className="row">
								<div className="small-12 column"><h5>Distribution Maps</h5></div>
							</div>
						</div>
						<div className="scroll-list-section-body">
							<div className="row scroll-list-header">
								<div className="hide-for-small-only medium-2 columns">
									{this.renderHeader('ClientName', 'Client Name')}
								</div>
								<div className="small-12 medium-5 columns">
									{this.renderHeader('ClientCode', 'Client Code')}
								</div>
								<div className="hide-for-small-only medium-2 columns">
									{this.renderHeader('Date', 'Date')}
								</div>
								<div className="small-12 medium-3 columns">
									<span className="show-for-large">
										{this.renderHeader('AreaDescription', 'Type')}
									</span>
								</div>
							</div>
							{list.map(function(item) {
					          	return (
					          		<DistributionRow key={item.get('Id')} model={item} />
				          		);
					        })}
				        </div>
				    </div>
		        </div>
			);
		}
	});
	return DistributionList;
});