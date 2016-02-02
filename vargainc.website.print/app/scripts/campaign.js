define([
	"jquery",
	'foundation',
	'alertify'
], function ($, foundation, alertify) {
	var options = {
		//Local dev
		// srcBaseUrl: "http://dev.timm.vargainc.com/",
		// targetBaseUrl: "http://dev.timm.vargainc.com/",
		// url: 'http://dev.timm.vargainc.com',
		// srcAPIVersion: "/",
		// targetBaseUrl: "//api/",
		// EC2
		//baseUrl: "http://52.8.230.211/201510/api/",
		url: '/',
		srcAPIVersion: "201510",
		targetBaseUrl: "../api/",
	};
	$(function () {
		$('#campaignCopyListDialog').foundation('reveal', 'open');
		$(document).on("click", "#btnQueryServer", function(){
			$('.overlayer').show();
			var checkAPIVersion = $('#txtQueryServer').val();
			if(!checkAPIVersion.length){
				alertify.error("please input the server address");
				$('.overlayer').hide();
				return;
			}
			var failed = function(){
				var container = $('#campaignCopyList tbody');
				container.empty();
				container.html('<tr><td colspan="3">Can not load campaign from server or not campaign exist in this server.</td></tr>');
			}
			$.ajax({
				url: options.url + checkAPIVersion + '/api/campaign',
				//url: options.url + checkAPIVersion + '/campaign',
				method: 'GET',
				dataType: "json",
				cache: false,
				success: function(result){
					if(result && result.length > 0){
						options.srcAPIVersion = checkAPIVersion;
						var container = $('#campaignCopyList tbody');
						container.empty();
						var html = '';
						$.each(result, function(){
							html += '<tr id="campaign-' + this.Id + '">';
							html += '<td class="text-center"><input id="selector-' + this.Id + '" type="radio" name="copyCampaign" campaignId="' + this.Id + '" /></td>';
							html += '<td><label for="selector-' + this.Id + '">' + this.Name + '</label></td>';
							html += '<td><label for="selector-' + this.Id + '">' + this.UserName + '</label></td>';
							html += '</tr>'
						});
						container.html(html);
						$('.campaignCopyListContainer').scrollTop(0);
					}else{
						failed();
					}
				},
				error: function(){
					failed();
				},
				complete: function(){
					$('.overlayer').hide();
				}
			});
		});
		$(document).on("click", "#btnCopy", function(){
			var copyFailed = function(){
				$('.overlayer').hide();
				alertify.error('copy campaign failed. please contact us!');
			};
			var selected = $('#campaignCopyList input:checked');
			if(selected.size() == 0){
				alertify.error('Please at least select an Campaign!');
				return;
			}
			$('.overlayer').show();
			var exportUrl = options.url + options.srcAPIVersion + '/api/campaign/' + selected.attr('campaignId') + '/export';
			//var exportUrl = options.url + options.srcAPIVersion + '/campaign/' + selected.attr('campaignId') + '/export';
			var importUrl = '../api/campaign/import';
			//var importUrl = options.url + '/campaign/import';
			$.getJSON(exportUrl).then(function(campaign){
				return $.post(importUrl, campaign, function(response) {
					if(response && response.success){
						alertify.success('copy success. please refresh control center!');
					}else{
						copyFailed();
					}
				});
			}, copyFailed).done(function(){
				$('.overlayer').hide();
			}).fail(copyFailed);
		});
		$(document).on('click', '.modal', function(){
			console.log('disable modal');
			return false;
		});
	});
});