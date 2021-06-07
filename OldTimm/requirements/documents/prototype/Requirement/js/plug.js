/*******************************************************************************
 * Plug submap relevant javascript objects in to other objects. 
 * 
 * The code in this file plays a role of glue, which assembles the submap 
 * relevant functionality with other functionality.
 ******************************************************************************/

// Inject 'on_picking_center'
function intercept_on_map_click_for_pick_center(
	realFunc, args, funcName) {
	var result = realFunc.apply(this, args);
	on_picking_center(args[5][0]);
	return result;
}

// Intercept function 'on_map_click'
Weaver.addAdvice(
	this, 
	'intercept_on_map_click_for_pick_center', 
	'around', 
	this, 
	'on_map_click');
