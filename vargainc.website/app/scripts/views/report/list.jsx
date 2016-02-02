define([
	'underscore',
	'moment',
	'backbone',
	'react',
	'pubsub',
	'models/task',
	'react.backbone'
], function (_, moment, Backbone, React, Topic, TaskModel) {
	var actionHandler = null;
	var ReportRow = React.createBackboneClass({
		menuKey: 'report-menu-ddl-',
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
		onReOpenTask: function(taskId){
			var confirmResult = confirm('Do you really want to move report back to GPS Montor?');
			if(confirmResult){
				var model = new TaskModel({Id:taskId});
				model.reOpen({
					success: function(result){
						if(result && result.success){
							Topic.publish('report/refresh');
						}else{
							alert(result.error);
						}
					}
				});
			}
		},
		onGotoReport: function(taskId){
			window.location.hash = 'frame/ReportsTask.aspx?tid=' + taskId;
		},
		onCloseMoreMenu: function(key){
			$("#" + this.menuKey + key).foundation('close');
		},
		renderMoreMenu: function(key){
			var id = this.menuKey + key;
			return (
				<span>
					<button className="button cirle" data-toggle={id}>
						<i className="fa fa-ellipsis-h"></i>
					</button>
					<div id={id} className="dropdown-pane bottom" 
						data-dropdown
						data-close-on-click="true" 
						data-auto-focus="false"
						onClick={this.onCloseMoreMenu.bind(null, key)}>
						<ul className="vertical menu">
							<li><a href="javascript:;" onClick={this.onEdit}><i className="fa fa-edit"></i><span>Edit</span></a></li>
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
					<div className="small-10 medium-5 columns">
						{model.get('ClientCode')}
					</div>
					<div className="small-2 medium-2 columns">
						{displayDate}
					</div>
					<div className="small-2 medium-3 columns">
						<span className="show-for-large">{model.get('AreaDescription')}</span>
						<div className="float-right tool-bar">
							<a  className="button row-button" href={"#frame/handler/PhantomjsPrintHandler.ashx?campaignId=" + model.get('Id') + "&type=print"}>
								<i className="fa fa-print"></i><small>Print</small>
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
								var taskDisplayDate = task && task.Date ? moment(task.Date).format("MMM DD, YYYY") : ''
								return (
									<tr key={task.Id}>
										<td onClick={self.onGotoReport.bind(null, task.Id)}>
											{taskDisplayDate} - {task.Name}
										</td>
										<td>
											<div className="float-right tool-bar">
												<a className="button row-button" href={"#frame/EditGTU.aspx?tid=" + task.Id}>
													<i className="fa fa-magic"></i><small>Review</small>
												</a>
												<button onClick={self.onReOpenTask.bind(null, task.Id)} 
													className="button has-tip top" title="dismiss" 
													data-tooltip aria-haspopup="true" 
													data-disable-hover='false' tabIndex="1">
													<i className="fa fa-reply"></i>
												</button>
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
	var ReportList = React.createBackboneClass({
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
			Topic.subscribe('report/refresh', function(){
				self.getCollection().fetchForReport();
			});
			Topic.subscribe('search', function(words){
				console.log('on search');
				self.setState({
					search: words,
					filterField: null,
					filterValues: []
				});
			});

			$("#report-filter-ddl-ClientName, #report-filter-ddl-ClientCode, #report-filter-ddl-Date, #report-filter-ddl-AreaDescription").foundation();
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
			$('#report-filter-ddl-' + field).foundation('close');
		},
		onClearFilter: function(field, e){
			e.preventDefault();
			e.stopPropagation();
			this.setState({
				filterField: null,
				filterValues: [],
				search: null
			});
			$('#report-filter-ddl-' + field).foundation('close');
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
					<a data-toggle={"report-filter-ddl-" + field}>
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
					<div id={"report-filter-ddl-" + field} 
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
					<a data-toggle={"report-filter-ddl-" + field}>
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
								<div className="small-12 column"><h5>Reports</h5></div>
							</div>
						</div>
						<div className="scroll-list-section-body">
							<div className="row scroll-list-header">
								<div className="hide-for-small-only medium-2 columns">
									{this.renderHeader('ClientName', 'Client Name')}
								</div>
								<div className="small-10 medium-5 columns">
									{this.renderHeader('ClientCode', 'Client Code')}
								</div>
								<div className="hide-for-small-only medium-2 columns">
									{this.renderHeader('Date', 'Date')}
								</div>
								<div className="small-2 medium-3 columns">
									<span className="show-for-large">
										{this.renderHeader('AreaDescription', 'Type')}
									</span>
								</div>
							</div>
							{list.map(function(item) {
					          	return (
					          		<ReportRow key={item.get('Id')} model={item} />
				          		);
					        })}
				        </div>
				    </div>
		        </div>
			);
		}
	});
	return ReportList;
});