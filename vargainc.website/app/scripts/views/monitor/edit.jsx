define([
	'moment',
	'backbone',
	'react',
	'pubsub',
	'react.backbone'
], function (moment, Backbone, React, Topic) {
	return React.createBackboneClass({
		componentDidMount: function(){
			var self = this,
				model = this.getModel();
			
			$('.fdatepicker').fdatepicker({
				format: 'yyyy-mm-dd'
			}).on('changeDate', function(e){
				console.log(e.date);
				self.getModel().set('Date', e.date);
			});
			$('form').foundation();
		},
		onSave: function(e){
			var self = this;
			this.getModel().save(null, {
				success: function(model, response){
					if(response && response.success){
						self.onClose();
					}else{
						self.setState({error: "opps! something wrong. please contact us!"});
					}
				},
				error: function(){
					self.setState({error: "opps! something wrong. please contact us!"});
				}
			});
			e.preventDefault();
			e.stopPropagation();
		},
		onClose: function(){
			$('.fdatepicker').off('changeDate').fdatepicker('remove');
			Topic.publish("showDialog");
		},
		onChange: function(e){
			console.log(e.currentTarget.name);
			this.getModel().set(e.currentTarget.name, e.currentTarget.value);
		},
		render: function(){
			var model = this.getModel();
			var date = model.get('Date');
			var displayDate = date ? moment(date).format("YYYY-MM-DD") : '';
			var tel = model.get('Telephone'),
				telphone,
				operator;
			if(tel){
				var telArray = tel.split('@');
				telphone = telArray[0];
				operator = '@' + telArray[1];
			}
			var showError = this.state && this.state.error ? true : false;
			var errorMessage = showError ? this.state.error : "";
			return (
				<form data-abide onSubmit={this.onSave}>
					<h3>Edit Task</h3>
					<div data-abide-error className="alert callout" style={{display: showError ? 'block' : 'none'}}>
				    	<p><i className="fa fa-exclamation-circle"></i>&nbsp;{errorMessage}</p>
				  	</div>
					<div className="row">
						<div className="small-12 columns">
							<label>Name
								<input type="text" value={model.get('Name')} readOnly />
							</label>
						</div>
						<div className="small-4 columns end">
							<label>Distribution Date
								<input className="fdatepicker" onChange={this.onChange} name="Date" type="date" readOnly value={displayDate} />
							</label>
						</div>
						<div className="small-12 columns">
							<label>Select Auditor
								<input type="text" defaultValue={model.get('AuditorName')} readOnly />
							</label>
						</div>
						<div className="small-12 columns">
							<label>Email
								<input onChange={this.onChange} name="Email" type="text" defaultValue={model.get('Email')} />
							</label>
						</div>
						<div className="small-12 columns">
							<label>Telephone
								<input onChange={this.onChange} name="Telephone" type="text" defaultValue={telphone} />
							</label>
						</div>
						<div className="small-12 columns">
							<label>Telecommunications Operator
								<select defaultValue={operator} onChange={this.onChange} name="Operator">
								    <option value="@message.alltel.com">Alltel</option>
				                    <option value="@txt.att.net">AT&amp;T</option>
				                    <option value="@messaging.nextel.com">Nextel</option>
				                    <option value="@messaging.sprintpcs.com">Sprint</option>
				                    <option value="@tms.suncom.com">SunCom</option>
				                    <option value="@tmomail.net">T-mobile</option>
				                    <option value="@voicestream.net">VoiceStream</option>
				                    <option value="@vtext.com">Verizon(text only)</option>
								</select>
							</label>
						</div>
					</div>
					<div className="small-12 columns">
						<div className="button-group float-right">
							<button type="submit" className="success button">Save</button>
							<a href="javascript:;" className="button" onClick={this.onClose}>Cancel</a>
						</div>
					</div>
					<button onClick={this.onClose} className="close-button" data-close aria-label="Close reveal" type="button">
				    	<span aria-hidden="true">&times;</span>
				  	</button>
				</form>
			);
		}
	});
});