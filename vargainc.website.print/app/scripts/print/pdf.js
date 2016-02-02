define([
	'jquery',
	'alertify'
], function ($, alertify) {
	var Pdf = function (options) {
		this.options = options;
	};
	$.extend(Pdf.prototype, {
		init: function () {
			$(document).on('click', '.editing', function () {
				return false;
			});
			$(document).on('click', '.editable', $.proxy(this.beginEdit, this));
			$(document).on('click', $.proxy(this.clearUp, this));
			$(document).on('keyup', '.editing', $.proxy(this.onEdit, this));
			var self = this;
			$(document).on('click', '.btnPrint', function () {
				if ($('.page-container .page:not(.hide) .map-container .loading').size() > 0) {
					alertify.alert("Loading, please wait.");
				} else {
					var options = self.buildExport();
					console.log('pdf', options);
					self.download(options);
				}

			});
		},
		clearUp: function () {
			var editEl = $(".editing")
			editEl.prev('.editable').show();
			editEl.remove();
		},
		beginEdit: function (event) {
			this.clearUp();
			var $this = $(event.target);
			var editor = $('<input class="editing" type="text">');
			editor.css({
				'width': ($this.parent().width() - 40) + 'px'
			}).insertAfter($this).val($this.text());
			if ($this.closest('div').hasClass('text-center')) {
				editor.addClass('text-center');
			}
			$this.hide();
			return false;
		},
		onEdit: function (event) {
			var editEl = $(event.target);
			if (event.which == 13) {
				editEl.prev('.editable').text(editEl.val()).show();
				editEl.remove();
			} else if (event.which == 27) {
				this.clearUp();
			} else {
				return false;
			}
		},
		buildExport: function () {
			var pdf = [],
				self = this;
			$('.page-container .page:not(.hide)').each(function () {
				var type = $(this).attr('role');
				var options = [];
				$('.row', this).each(function () {
					var item = {};
					if ($(this).attr('role') == 'list') {
						var list = [];
						var listRow = $('[role]', this);
						for (var i = 0; i < listRow.size(); i++) {
							list.push({
								key: listRow.eq(i).text(),
								text: listRow.eq(++i).text()
							});
						}
						item['list'] = list;
					} else if ($(this).attr('role') == 'table') {
						item['table'] = $(this).attr('datasource');
						var page = $(this).parents('.page');
						if (page.data('submapId')) {
							item['submapId'] = page.data('submapId');
						}
						if (page.data('dmapId')) {
							item['dmapId'] = page.data('dmapId');
						}
					} else {
						$('[role]', this).each(function () {
							var role = $(this).attr('role');
							console.log(role);
							if (role == 'image') {
								item[role] = $(this).attr('src');
								item['height'] = $(this).attr('p-height');
								item['align'] = $(this).attr('p-align');
								item['top'] = $(this).attr('p-top');
								item['bottom'] = $(this).attr('p-bottom');
							} else if (role == 'map') {
								item[role] = $(this).find('img').attr('src');
								item['legend'] = $(this).attr('legend');
								if($(this).attr('color') == 'true'){
									item['color'] = self.options.PenetrationColors;
								}
								var bg = $(this).css('background-image').replace(/"/g, '');
								bg = bg.substr(4);
								item["bg"] = bg.substring(0, bg.length - 1);
							} else {
								item[role] = $(this).text();
							}
						});
					}
					options.push(item);
				});

				pdf.push({
					type: type,
					options: options
				});
			});
			return pdf;
		},
		download: function (params) {
			$('.overlayer').show();
			var postData = {
				campaignId: this.options.campaignId,
				size: $('.page-container').hasClass('dmapPrint') ? "Distribute": "A4",
				needFooter: !$('.page-container').hasClass('dmapPrint'),
				options: params
			};
			var url = this.options.baseUrl + 'pdf/build/';
			var self = this;
			$.ajax({
				url: url,
				method: 'POST',
				data: {options: JSON.stringify(postData)},
				success: function(response){
					if(response){
						var downloadUrl = self.options.baseUrl + 'pdf/download/' + self.options.campaignId + '/' + response.sourceFile;
						$('<form action="' + downloadUrl + '" method="GET"></form>').appendTo('body').get(0).submit();
					}
				},
				complete: function(){
					$('.overlayer').hide();
				}
			});
		}
	});
	return Pdf;
});