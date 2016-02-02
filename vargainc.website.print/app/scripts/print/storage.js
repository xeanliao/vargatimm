define([
	'jquery',
	'alertify',
	'localforage',
], function ($, alertify, localforage) {

	function Storage(baseUrl, campaignId) {
		this.baseUrl = baseUrl;
		this.campaignId = campaignId;
		this.structure = {};
		console.log(this.baseUrl);
		this.localStorage = localforage.createInstance({
			name: "CampaignPrint" + campaignId
		});
		this.localStorage.clear();
	}

	$.extend(Storage.prototype, {
		clearAll: function () {},
		loadCampaign: function () {
			console.log("begin load campaign");
			var def = $.Deferred();
			var self = this;
			this.localStorage.getItem("Campaign", function (error, val) {
				if (!error && val) {
					def.resolve(val);
				} else {
					var url = self.baseUrl + "print/campaign/" + self.campaignId;
					$.getJSON(url, function (result) {
						self.localStorage.setItem("Campaign", result, function () {
							def.resolve(result);
						});
					}, function () {
						alertify.error("locad campaign info failed. please contact us!");
						def.reject();
					});
				}
			});

			return def;
		},
		loadSubMapBundary: function (submapId) {
			var def = $.Deferred();
			$.ajax({
				url: this.baseUrl + "print/campaign/" + this.campaignId + "/submap/" + submapId + '/bundary',
				method: "GET",
				success: function (result) {
					def.resolve(result);
				},
				error: function () {
					def.reject();
				}
			});
			return def;
		},
		loadSubMapDetail: function (submapId) {
			var def = $.Deferred();
			$.ajax({
				url: this.baseUrl + "print/campaign/" + this.campaignId + "/submap/" + submapId + '/record',
				method: "GET",
				success: function (result) {
					def.resolve(result);
				},
				error: function () {
					def.reject();
				}
			});
			return def;
		},
		loadDMapBundary: function (submapId, dmapId) {
			var def = $.Deferred();
			$.ajax({
				url: this.baseUrl + "print/campaign/" + this.campaignId + "/submap/" + submapId + '/dmap/' + dmapId + '/bundary',
				method: "GET",
				success: function (result) {
					def.resolve(result);
				},
				error: function () {
					def.reject();
				}
			});
			return def;
		},
		getCampaignBundary: function () {
			var def = $.Deferred();
			var self = this;
			this.localStorage.getItem("Campaign", function (error, result) {
				var query = [];
				$.each(result.SubMaps, function () {
					query.push(self.getSubMapBundary(this.Id));
				});
				$.when.apply(self, query).done(function () {
					var args = Array.prototype.slice.call(arguments);
					def.resolve(args);
				});
			});
			return def;
		},
		getSubMapBundary: function (submapId) {
			var def = $.Deferred();
			var self = this;
			this.localStorage.getItem("SubMapBundary_" + submapId, function (error, result) {
				if (error || result == null) {
					self.loadSubMapBundary(submapId).done(function (result) {
						self.localStorage.setItem("SubMapBundary_" + submapId, result);
						def.resolve(result);
					});
				} else {
					def.resolve(result);
				}
			});
			return def;
		},
		getDMapBundary: function (submapId, dmapId) {
			var def = $.Deferred();
			var self = this;
			this.localStorage.getItem("DMapBundary_" + dmapId, function (error, result) {
				if (error || result == null) {
					self.loadDMapBundary(submapId, dmapId).done(function (result) {
						self.localStorage.setItem("DMapBundary_" + dmapId, result);
						def.resolve(result);
					});
				} else {
					def.resolve(result);
				}
			});
			return def;
		}
	});
	return Storage;
});