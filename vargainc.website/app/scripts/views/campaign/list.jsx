define([
	'underscore',
	'moment',
	'backbone',
	'react',
	'views/base',
	'views/campaign/edit',
	'views/campaign/publish',
	'models/campaign',
	'react.backbone'
], function (_, moment, Backbone, React, BaseView, EditView, PublishView, Model) {
	var CampaignRow = React.createBackboneClass({
		mixins: [BaseView],
		menuKey: 'campaign-menu-ddl-',
		getDefaultProps: function(){
			return {
				address: null,
				icon: null,
				name: null
			};
		},
		componentDidMount: function(){
			$("#" + this.menuKey + this.getModel().get('Id')).foundation();
		},
		componentDidUpdate: function(){
		},
		onCopy: function(e){
			e.preventDefault();
			e.stopPropagation();
			$(e.currentTarget).closest('.dropdown-pane').foundation('close');
			var model = this.getModel(),
				self = this;
			model.copy({
				success: function(response){
					if(response && response.success){
						var copiedModel = new Model(response.data);
						model.collection.add(copiedModel, {at: 0});
					}
				},
				error: function(){
					
				}
			});
		},
		onEdit: function (e) {
			e.preventDefault();
			e.stopPropagation();
			$(e.currentTarget).closest('.dropdown-pane').foundation('close');
			this.publish('showDialog', EditView, {
				model: this.getModel().clone()
			});
		},
		onDelete: function (e) {
			e.preventDefault();
			e.stopPropagation();
			$(e.currentTarget).closest('.dropdown-pane').foundation('close');
			var self = this;
			var result = confirm('are you sure want delete this campaign?');
			if (result) {
				var model = self.getModel();
				model.destroy({
					wait: true,
					success: function () {
						self.publish('camapign/refresh');
					}
				});
			}
		},
		onPublishToDMap: function (e) {
			e.preventDefault();
			e.stopPropagation();
			var model = this.getModel(),
				self = this;
			this.publish('showDialog', PublishView);

			this.unsubscribe('campaign/publish');
			this.subscribe('campaign/publish', function (user) {
				model.publishToDMap(user, {
					success: function (result) {
						self.publish('showDialog');
						if (result && result.success) {
							self.publish('camapign/refresh');
						} else {
							alert(result.error);
						}
					}
				});
			});
		},
		onOpenMoreMenu: function(e){
			e.preventDefault();
			e.stopPropagation();
		},
		onCloseMoreMenu: function(key, e){
			e.preventDefault();
			e.stopPropagation();
			$("#" + this.menuKey + key).foundation('close');
		},
		renderMoreMenu: function(key){
			var id = this.menuKey + key;
			return (
				<span>
					<button className="button cirle" data-toggle={id} onClick={this.onOpenMoreMenu}>
						<i className="fa fa-ellipsis-h"></i>
					</button>
					<div id={id} className="dropdown-pane bottom" 
						data-dropdown
						data-close-on-click="true" 
						data-auto-focus="false"
						onClick={this.onCloseMoreMenu.bind(null, key)}>
						<ul className="vertical menu">
							<li><a href="javascript:;" onClick={this.onEdit}><i className="fa fa-edit"></i><span>Edit</span></a></li>
							<li><a href="javascript:;" onClick={this.onCopy}><i className="fa fa-copy"></i><span>Copy</span></a></li>
							<li><a href="javascript:;" onClick={this.onDelete}><i className="fa fa-trash"></i><span>Delete</span></a></li>
						</ul>
					</div>
				</span>
			);
		},
		onGotoCMap: function(id){
			window.location.hash = 'frame/campaign.aspx?cid=' + id;
		},
		render: function(){
			var model = this.getModel();
			var date = model.get('Date');
			var displayDate = date ? moment(date).format("MMM DD, YYYY") : '';
			return (
				<div className="row scroll-list-item" onDoubleClick={this.onGotoCMap.bind(null, model.get('Id'))}>
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
							<button onClick={this.onPublishToDMap} className="button">
								<i className="fa fa-upload"></i><small>Publish</small>
							</button>
							{this.renderMoreMenu(model.get('Id'))}
						</div>
					</div>
				</div>
			);
		}
	});

	return React.createBackboneClass({
		mixins: [BaseView],
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
			this.subscribe('camapign/refresh', function(){
				self.getCollection().fetch();
			});
			this.subscribe('search', function(words){
				self.setState({
					search: words,
					filterField: null,
					filterValues: []
				});
			});

			$("#campaign-filter-ddl-ClientName, #campaign-filter-ddl-ClientCode, #campaign-filter-ddl-Date, #campaign-filter-ddl-AreaDescription").foundation();
		},
		onNew: function () {
			this.publish('showDialog', EditView, {
				model: new Model()
			});
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
			$('#campaign-filter-ddl-' + field).foundation('close');
		},
		onClearFilter: function(field, e){
			e.preventDefault();
			e.stopPropagation();
			this.setState({
				filterField: null,
				filterValues: [],
				search: null
			});
			$('#campaign-filter-ddl-' + field).foundation('close');
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
					<a data-toggle={"campaign-filter-ddl-" + field}>
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
					<div id={"campaign-filter-ddl-" + field} 
						className="dropdown-pane bottom" 
						style={{'width': 'auto'}}
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
					<a data-toggle={"campaign-filter-ddl-" + field}>
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
							<div className="row" data-equalizer>
								<div className="small-4 column"><h5>Campaign</h5></div>
								<div className="small-8 column">
									<button onClick={this.onNew} className="float-right section-button">
										<i className="fa fa-plus"></i><span>New Campaign</span>
									</button>
								</div>
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
					          		<CampaignRow key={item.get('Id')} model={item} />
				          		);
					        })}
				        </div>
				    </div>
		        </div>
			);
		}
	});
});