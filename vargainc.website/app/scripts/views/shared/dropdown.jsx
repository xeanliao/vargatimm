export default React.createClass({
	render: function(){
		return (
			<ul className="dropdown menu is-dropdown-submenu-parent opens-right is-active">
				<li>
					<a href="#">Switch Active Task</a>
					<ul className="menu submenu is-dropdown-submenu first-sub vertical js-dropdown-active">
						<li><a href="#">Item 1A</a></li>
					</ul>
				</li>
			</ul>
		);
	}
});