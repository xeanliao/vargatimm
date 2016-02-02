define(['underscore', 'react'], function (_, React) {
	return React.createClass({
		render: function () {
			return React.createElement(
				'div',
				{ className: 'section row' },
				React.createElement(
					'div',
					{ className: 'small-12 columns' },
					React.createElement(
						'div',
						{ className: 'section-header' },
						React.createElement(
							'div',
							{ className: 'row' },
							React.createElement(
								'div',
								{ className: 'small-12 column' },
								React.createElement(
									'h5',
									null,
									'Administration'
								)
							)
						)
					)
				),
				React.createElement(
					'div',
					{ className: 'small-12 columns' },
					React.createElement(
						'div',
						{ className: 'row', style: { 'marginTop': '120px' } },
						React.createElement(
							'div',
							{ className: 'small-7 small-centered' },
							React.createElement(
								'div',
								{ className: 'row' },
								React.createElement(
									'div',
									{ className: 'small-12 column' },
									React.createElement(
										'div',
										{ className: 'callout secondary' },
										React.createElement(
											'a',
											{ href: '#frame/Users.aspx' },
											React.createElement(
												'span',
												null,
												'User Management'
											)
										)
									)
								)
							),
							React.createElement(
								'div',
								{ className: 'row small-up-2 medium-up-2 large-up-2' },
								React.createElement(
									'div',
									{ className: 'column' },
									React.createElement(
										'div',
										{ className: 'callout secondary' },
										React.createElement(
											'a',
											{ href: '#frame/NonDeliverables.aspx' },
											React.createElement(
												'span',
												null,
												'Non-Deliverables'
											)
										)
									)
								),
								React.createElement(
									'div',
									{ className: 'column' },
									React.createElement(
										'div',
										{ className: 'callout secondary' },
										React.createElement(
											'a',
											{ href: '#frame/GtuAdmin.aspx?AssignNameToGTUFlag=true' },
											React.createElement(
												'span',
												null,
												'GTU Management'
											)
										)
									)
								),
								React.createElement(
									'div',
									{ className: 'column' },
									React.createElement(
										'div',
										{ className: 'callout secondary' },
										React.createElement(
											'a',
											{ href: '#frame/AvailableGTUList.aspx' },
											React.createElement(
												'span',
												null,
												'GTU Available List'
											)
										)
									)
								),
								React.createElement(
									'div',
									{ className: 'column' },
									React.createElement(
										'div',
										{ className: 'callout secondary' },
										React.createElement(
											'a',
											{ href: '#frame/AdminGtuToBag.aspx' },
											React.createElement(
												'span',
												null,
												'GTU bag Management '
											)
										)
									)
								),
								React.createElement(
									'div',
									{ className: 'column' },
									React.createElement(
										'div',
										{ className: 'callout secondary' },
										React.createElement(
											'a',
											{ href: '#frame/AdminGtuBagToAuditor.aspx' },
											React.createElement(
												'span',
												null,
												'Assign GTU-Bags to Auditors'
											)
										)
									)
								),
								React.createElement(
									'div',
									{ className: 'column' },
									React.createElement(
										'div',
										{ className: 'callout secondary' },
										React.createElement(
											'a',
											{ href: '#frame/AdminDistributorCompany.aspx' },
											React.createElement(
												'span',
												null,
												'Distributor Management'
											)
										)
									)
								)
							)
						)
					)
				)
			);
		}
	});
});
