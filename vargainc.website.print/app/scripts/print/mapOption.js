define([
	'jquery',
	'alertify',
	'jquery-ui'
], function ($, alertify) {
	return {
		timeout: null,
		ele: null,
		target: null,
		options: null,
		key: 'MapOptions',
		init: function (options) {
			var self = this;
			this.ele = $('.mapOption');
			this.options = options;

			// $(document).on('mouseenter', '.map-container', function () {
			// 	$(document).off('click.map.option');
			// 	self.show.call(self, $(this));
			// 	$(document).one('click.map.option', 'body', function () {
			// 		self.hide.call(self);
			// 	});
			// });
			$(document).on('click', '.map-container', function () {
				$(document).off('click.map.option');
				self.show($(this));
				$(document).one('click.map.option', 'body', function () {
					self.hide.call(self);
				});
				return false;
			});
			$(document).on('click', '.mapOption', function () {
				return false;
			});
			// $(document).on('mouseleave', '.map-container, .mapOption', function () {
			// 	self.timeout = window.setTimeout(function () {
			// 		self.hide.call(self);
			// 	}, 300);
			// });
			// $(document).on('mouseenter', '.mapOption', function () {
			// 	window.clearTimeout(self.timeout);
			// });
			$(document).on('scroll', function () {
				if (self.target) {
					self.ele.css('top', self.calcPosition());
				}
			});
			$(document).on('click', '.btnMapOptionApply', function () {
				self.apply.call(self);
				self.hide.apply(self);
			});
			$(document).on('click', '.btnMapOptionCancel', function () {
				self.hide.apply(self);
			});
			$(document).on('click', '.btnMapImageDownload', function () {
				if (self.target) {
					var mapContainer = self.target;
					var bg = mapContainer.css('background-image').replace(/"/g, '');
					bg = bg.substr(4);
					bg = bg.substring(0, bg.length - 1);
					var map = mapContainer.find('img').attr('src');
					var downloadUrl = self.options.baseUrl + 'pdf/download/images';
					if(map){
						var form = $('<form action="' + downloadUrl + '" method="POST"></form>').appendTo('body');
						form.append('<input type="hidden" name="foreground" value="' + map + '" />');
						form.append('<input type="hidden" name="background" value="' + bg + '" />');
						form.append('<input type="hidden" name="width" value="' + bg + '" />');
						form.append('<input type="hidden" name="height" value="' + bg + '" />');
						form.get(0).submit();
					}
				}
			});
			$(document).on('click', '.btnRoadMap, .btnHybrid', function () {
				self.ele.find('.selected').removeClass('selected');
				$(this).addClass('selected');
			});
			$('#zoomSlider').slider({
				min: 0,
				max: 18,
				step: 1,
				change: function(event, ui) {
					self.zoomChanged = true;
					if(ui.value == 0){
						$(this).parents('label').find('span').eq(0).text('Auto');
					}else{
						$(this).parents('label').find('span').eq(0).text(ui.value);
					}
				}
			});
			$('#gtuSlider').slider({
				min: 2,
				max: 32,
				step: 1,
				change: function(event, ui) {
					self.gtuChanged = true;
					$(this).parents('label').find('span').eq(0).text(ui.value);
				}
			});
		},
		calcPosition: function () {
			if (this.target) {
				var fix = 60,
					targetTop = this.target.offset().top,
					targetHeight = this.target.height(),
					optionHeight = this.ele.height(),
					targetBottom = targetTop + targetHeight - optionHeight - fix,
					scrollBottom = $(window).scrollTop() + $(window).height() - optionHeight - fix;
				return Math.max(Math.min(targetBottom, scrollBottom), targetTop);
			}
			return null;
		},
		show: function (target) {
			window.clearTimeout(this.timeout);
			if (this.target == null || target.parents('.page').attr('id') != this.target.parents('.page').attr('id')) {	
				console.log('show map option');
				var page = target.parents(".page"),
					options = $.extend({}, this.options, page.data(this.key));
				this.ele.find('th').removeClass('selected');
				this.ele.find('[mapType]').removeClass('selected');
				this.ele.find('[mapType="' + options.mapType + '"]').addClass('selected');

				if (page.hasClass('dmap') && $('.page-container').hasClass('dmapPrint')) {
					this.ele.find('.zoom').show();
					this.ele.find('.gtu').hide();
					this.ele.find('#zoomSlider').slider('value',  options.zoom);
					this.ele.find('.btnMapImageDownload').show();
				} else if (page.hasClass('dmap') && !$('.page-container').hasClass('dmapPrint')) {
					this.ele.find('.zoom').hide();
					this.ele.find('.gtu').show();
					this.ele.find('#zoomSlider').slider('value',  options.gTUDotsRadii);
					this.ele.find('.btnMapImageDownload').hide();
				}else{
					this.ele.find('.zoom').hide();
					this.ele.find('.gtu').hide();
					this.ele.find('.btnMapImageDownload').hide();
				}
			}
			this.target = target;
			this.ele.removeClass('hide').css('top', this.calcPosition());
		},
		apply: function () {
			var newOption = {};
			newOption.mapType = this.ele.find('.selected').attr('mapType');
			if (this.ele.find('.zoom').css('display') != 'none') {
				newOption.zoom = parseInt(this.ele.find('#zoomSlider').slider('value'));
			}
			if (this.ele.find('.gtu').css('display') != 'none') {
				newOption.gTUDotsRadii = parseInt(this.ele.find('#gtuSlider').slider('value'));
			}
			this.target.parents('.page').data(this.key, newOption);
			this.target.trigger('retry.click');
			this.hide();
		},
		hide: function () {
			$(document).off('click.map.option');
			if (this.target) {
				this.ele.addClass('hide');
			}
			this.target = null;
		}
	};
});