define(['backbone', 'react', 'react.backbone'], function (Backbone, React) {
	return React.createBackboneClass({
		mixins: [React.BackboneMixin("baseInfo"), React.BackboneMixin("userInfo"), React.BackboneMixin("menuInfo")],
		render: function () {
			return React.createElement(
				'div',
				{ className: 'off-canvas-wrapper' },
				React.createElement(
					'div',
					{ className: 'off-canvas-wrapper-inner', 'data-off-canvas-wrapper': true },
					React.createElement(
						'div',
						{ className: 'off-canvas position-left reveal-for-large', 'data-off-canvas': true, 'data-position': 'left' },
						React.createElement(SidebarView, null)
					),
					React.createElement(
						'div',
						{ className: 'off-canvas-content', 'data-off-canvas-content': true },
						React.createElement(
							'div',
							{ className: 'title-bar' },
							React.createElement(
								'div',
								{ className: 'title-bar-left' },
								React.createElement('button', { 'aria-controls': 'my-info', 'aria-expanded': 'true', className: 'menu-icon', type: 'button', 'data-open': 'leftSideBar' }),
								React.createElement(
									'span',
									{ className: 'title-bar-title' },
									'TIMM'
								)
							)
						),
						React.createElement(MainView, null)
					)
				),
				React.createElement(OutsideView, null)
			);
		}
	});
});
//# sourceMappingURL=view.js.map
