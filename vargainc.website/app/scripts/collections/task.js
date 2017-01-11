import Backbone from 'backbone';
import TaskModel from 'models/task';

export default Backbone.Collection.extend({
	model: TaskModel,
});