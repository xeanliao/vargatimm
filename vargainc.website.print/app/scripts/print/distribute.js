define([
	'jquery',
	'metisMenu',
	'alertify',
	'print/campaign',
	'print/storage',
	'handlebars',
	'templates/print/footer',
	'templates/print/cover',
	'templates/print/distribute',
	'templates/print/tableOfContents',
	'templates/print/optionDialog'
], function($, metisMenu, alertify, Campaign, Storage, Handlebars, FooterTemplate, CoverTemplate, DistributeTemplate, TableOfContentsTemplate, optionDialogTemplate){
	function Distribute(campaignId, options) {
		this.campaignId = campaignId;
		$.extend(this.options, options);
		this.storage = new Storage(this.options.baseUrl, this.campaignId);
	};
	$.extend(Distribute.prototype, Campaign.prototype, {
		initPage: function(){
			console.log("onCampaignLoaded");
			var self = this;
			this.storage.loadCampaign().done(function (val) {
				self.campaign = val;
				
				Handlebars.registerPartial('footer', FooterTemplate);

				var pageContainer = $('.page-container');
				pageContainer.empty();

				/**
				 * build cover campaign campaign summary
				 */
				if (!self.options.suppressCover) {
					var cover = CoverTemplate(val);
					$('<div id="cover" class="page cover" role="cover">').html(cover).appendTo(pageContainer);
				}
				// for render option dialog
				var allDmaps = [];
				/**
				 * build dmap
				 */
				$.each(val.SubMaps, function () {
					var subMap = this;
					$.each(this.DMaps, function () {
						allDmaps.push({
							Id: this.Id,
							Name: this.Name
						});

						if (self.options.distributorMap.length == 0 || $.inArray(this.Id + '', self.options.distributorMap) == -1) {
							var dmapValue = $.extend({}, val, {
								submap: subMap,
								dmap: this
							});
							var dmap = DistributeTemplate(dmapValue);
							var dmapContainer = $('<div id="dmap-' + this.Id + '" class="page dmap" role="dmap">').html(dmap)
								.appendTo(pageContainer)
								.data('submapId', subMap.Id)
								.data('dmapId', this.Id);
						}
					});
				});

				/**
				 * build table of contents
				 */
				var tableOfContents = TableOfContentsTemplate(val);
				$(".table-of-contents").html(tableOfContents);
				$(".table-of-contents").metisMenu();

				$("#btnPrint").removeClass('disabled');
				alertify.success("Map images loading, this may take a couple of minutes.");

				$(document).trigger("print.loadMapImage");
			});
		},
		onLoadDMapImage: function (options, name) {
			options.type = 'Distribute';
			$.ajax({
				url: options.mapUrl,
				method: "POST",
				async: true,
				data: options,
				success: function (result) {
					console.log('load dmap image success');
					if (result && result.success && result.dmapId) {
						console.log(result.dmapId);
						var container = $('#dmap-' + result.dmapId + ' .map-container');
						var tiles = options.imgUrl + result.tiles;
						var geometry = '<img src="' + options.imgUrl + result.geometry + '" />'
						container.removeClass("loading hexdots-loader").html(geometry);
						container.css({
							'background-image': 'url(' + tiles + ')',
							'background-repeat': 'no-repeat',
							'background-size': '100% auto',
							'background-position': '0 0'
						});
						alertify.success('Distribute map ' + name + ' map image load success!');
					} else {
						$('#dmap-' + options.dmapId + ' .map-container').html('<div class="btnRetry"><i class="fa fa-refresh fa-5x"></i></div>');
						alertify.error('Distribute map ' + name + ' map image load failed. please try again later!');
					}
				},
				error: function (e) {
					$('#dmap-' + options.dmapId + ' .map-container').html('<div class="btnRetry"><i class="fa fa-refresh fa-5x"></i></div>');
					alertify.error('Distribute map ' + name + ' map image load failed. please try again later!');
				}
			});
		},
	});
	return Distribute;
});