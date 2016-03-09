define([
	'jquery',
	'underscore',
	'moment',
	'backbone',
	'react',
	'views/base',
	'views/monitor/dismiss',
	'views/monitor/edit',
	'models/task',
	'react.backbone'
], function ($, _, moment, Backbone, React, BaseView, DismissView, EditView, TaskModel) {
	var MonitorRow = React.createBackboneClass({
		mixins: [BaseView],
		menuKey: 'monitor-menu-ddl-',
		getDefaultProps: function(){
			return {
				address: null,
				icon: null,
				name: null
			};
		},
		componentDidMount: function(){
			var menuKey = this.menuKey;
			_.forEach(this.getModel().get('Tasks'), function (task) {
				$("#" + menuKey + task.Id).foundation();
			});
		},
		componentDidUpdate: function(){
		},
		onDismiss: function(e){
			e.preventDefault();
			e.stopPropagation();
			var model = this.getModel(),
				self = this,
				result = confirm('Are you sure you want to delete all selected Tasks?');
			if(result){
				this.publish('showDialog', DismissView);
				this.unsubscribe('monitor/dismiss');
				this.subscribe('monitor/dismiss', function(user){
					model.dismissToDMap(user, {
						success: function(result){
							if(result && result.success){
								self.publish('monitor/refresh');
							}else{
								alert("something wrong");
							}
						},
						complete: function(){
							self.publish('showDialog');
						}
					});
				});
			}
		},
		onFinished: function(taskId, reactEvent, reactNumber, evt){
			evt.preventDefault();
			evt.stopPropagation();

			var model = new TaskModel({Id:taskId}),
				self = this;
			model.markFinished({
				success: function(result){
					if(result && result.success){
						self.publish('monitor/refresh');
					}else{
						alert(result.error);
					}
				}
			});
		},
		onOpenUploadFile: function(taskId){
			$("#upload-file-" + taskId).click();
		},
		onImport: function (taskId, e) {
			$(e.currentTarget).closest('.dropdown-pane').foundation('close');
			e.bubbles = false;

			var uploadFile = e.currentTarget.files[0];
			if (uploadFile.size == 0) {
				alert('please select an not empty file!');
				return;
			}

			var model = new TaskModel({
					Id: taskId
				}),
				self = this;

			model.importGtu(uploadFile, {
				success: function (result) {
					$("#upload-file-" + taskId).val('');
					if (result && result.success) {
						self.publish('monitor/refresh');
					}
					if (result && result.error && result.error.length > 0) {
						alert(result.error.join('\r\n'));
					}
				}
			});
		},
		onEdit: function (taskId, e) {
			e.preventDefault();
			e.stopPropagation();
			$(e.currentTarget).closest('.dropdown-pane').foundation('close');

			var model = new TaskModel({
					Id: taskId
				}),
				self = this;

			model.fetch().then(function () {
				self.publish('showDialog', EditView, {
					model: model
				});
			});
		},
		onGotoMonitor: function(taskId, e){
			window.location.hash = 'frame/taskMonitor.aspx?taskid=' + taskId;
		},
		onOpenMoreMenu: function(e){
			e.preventDefault();
			e.stopPropagation();
		},
		onCloseMoreMenu: function(key){
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
							<li><a href="javascript:;" onClick={this.onEdit.bind(null, key)}><i className="fa fa-edit"></i><span>Edit</span></a></li>
							<li><a href="javascript:;" onClick={this.onOpenUploadFile.bind(null, key)}><i className="fa fa-cloud-upload"></i><span>Import</span></a></li>
							<input type="file" id={'upload-file-' + key} multiple style={{'display':'none'}} onChange={this.onImport.bind(null, key)} />
						</ul>
					</div>
				</span>
			);
		},
		render: function(){
			var model = this.getModel(),
				date = model.get('Date'),
				displayDate = date ? moment(date).format("MMM DD, YYYY") : '',
				self = this;
			return (
				<div className="row scroll-list-item">
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
							<a onClick={this.onDismiss} className="button row-button">
								<i className="fa fa-reply"></i><small>Dismiss</small>
							</a>
						</div>
					</div>
					<div className="small-12 columns">
						<table className="hover">
							<colgroup>
								<col />
								<col style={{'width': "160px"}} />
							</colgroup>
							<tbody>
							{model.get('Tasks').map(function(task){
								if (task.visiable === false) {
									return null;
								}
								return (
									<tr key={task.Id}>
										<td onDoubleClick={self.onGotoMonitor.bind(null, task.Id)}>
											{task.Name}
										</td>
										<td>
											<div className="float-right tool-bar">
												<button onClick={self.onFinished.bind(null, task.Id)} className="button">
													<i className="fa fa-check"></i><small>Finish</small>
												</button>
												{self.renderMoreMenu(task.Id)}
											</div>
										</td>
									</tr>
								);
							})}
							</tbody>
						</table>
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
			this.subscribe('monitor/refresh', function(){
				self.getCollection().fetchForTask();
			});
			this.subscribe('search', function(words){
				self.setState({
					search: words,
					filterField: null,
					filterValues: []
				});
			});

			$("#monitor-filter-ddl-ClientName, #monitor-filter-ddl-ClientCode, #monitor-filter-ddl-Date, #monitor-filter-ddl-AreaDescription").foundation();
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
			$('#monitor-filter-ddl-' + field).foundation('close');
		},
		onClearFilter: function(field, e){
			e.preventDefault();
			e.stopPropagation();
			$('#monitor-filter-ddl-' + field).foundation('close');
			$(e.currentTarget).closest('form').get(0).reset();

			this.setState({
				filterField: null,
				filterValues: [],
				search: null
			});
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
					<a data-toggle={"monitor-filter-ddl-" + field}>
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
					<div id={"monitor-filter-ddl-" + field} 
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
					<a data-toggle={"monitor-filter-ddl-" + field}>
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
					campaignValues = null,
					campaignSearch = null,
					taskValues = null,
					taskSearch = null;
				dataSource = _.filter(dataSource, function(i) {
					campaignValues  = _.values(i.attributes);
					campaignSearch = _.some(campaignValues, function(i){
						var dateCheck = moment(i, moment.ISO_8601);
						if(dateCheck.isValid()){
							return dateCheck.format("MMM DD YYYY MMM DD, YYYY YYYY-MM-DD MM/DD/YYYY YYYY MM MMM DD").toLowerCase().indexOf(keyword) > -1;
						}
						return _.toString(i).toLowerCase().indexOf(keyword) > -1;
					});
					/**
					 * update task visiable logical.
					 * if campaign in search keyward. show all task
					 * otherwise only show task name in search word.
					 * if there is no task need show hide this campaign.
					 */
					taskValues = _.values(i.attributes.Tasks);
					_.forEach(taskValues, function(i){
						i.visiable = campaignSearch || i.Name.toLowerCase().indexOf(keyword) > -1;
					});
					return campaignSearch || _.some(taskValues, {visiable:true});
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
								<div className="small-12 column"><h5>GPS Monitor</h5></div>
								<div className="small-12 column">
									<nav aria-label="You are here:" role="navigation">
										<ul className="breadcrumbs">
											<li><a href="#">Control Center</a></li>
											<li>
												<span className="show-for-sr">Current: </span> GPS Monitor
											</li>
										</ul>
									</nav>
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
					          		<MonitorRow key={item.get('Id')} model={item} />
				          		);
					        })}
				        </div>
				    </div>
		        </div>
			);
		}
	});
});