define([
	'underscore',
	'react'
], function (_, React) {
	return React.createClass({
		render: function(){
			return (
				<div className="section row">
					<div className="small-12 columns">
						<div className="section-header">
							<div className="row">
								<div className="small-12 column"><h5>Administration</h5></div>
								<div className="small-12 column">
									<nav aria-label="You are here:" role="navigation">
										<ul className="breadcrumbs">
											<li><a href="#">Control Center</a></li>
											<li>
												<span className="show-for-sr">Current: </span> Administration
											</li>
										</ul>
									</nav>
								</div>
							</div>
						</div>
					</div>
					<div className="small-12 columns">
						<div className="row" style={{'marginTop': '120px'}}>
							<div className="small-7 small-centered">
								<div className="row">
									<div className="small-12 column">
										<div className="callout secondary"><a href="#frame/Users.aspx"><span>User Management</span></a></div>
									</div>
									<div className="small-12 column">
										<div className="callout secondary"><a href="#campaign/import"><span>Import Company</span></a></div>
									</div>
								</div>
								<div className="row small-up-2 medium-up-2 large-up-2">
									<div className="column"><div className="callout secondary"><a href="#frame/NonDeliverables.aspx"><span>Non-Deliverables</span></a></div></div>
									<div className="column"><div className="callout secondary"><a href="#frame/GtuAdmin.aspx?AssignNameToGTUFlag=true"><span>GTU Management</span></a></div></div>
									<div className="column"><div className="callout secondary"><a href="#admin/gtu"><span>GTU Available List</span></a></div></div>
									<div className="column"><div className="callout secondary"><a href="#frame/AdminGtuToBag.aspx"><span>GTU bag Management </span></a></div></div>
									<div className="column"><div className="callout secondary"><a href="#frame/AdminGtuBagToAuditor.aspx"><span>Assign GTU-Bags to Auditors</span></a></div></div>
									<div className="column"><div className="callout secondary"><a href="#frame/AdminDistributorCompany.aspx"><span>Distributor Management</span></a></div></div>
								</div>
							</div>
						</div>
					</div>
				</div>
			);
		}
	});
});