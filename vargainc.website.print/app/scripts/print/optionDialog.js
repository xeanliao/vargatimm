define([
	'jquery'
], function () {
	return {
		haveInit: false,
		init: function () {
			console.log('first init option dialog');
			$(document).on('click', '.btnCheckAllDMap', function () {
				var status = $(this).prop('checked');
				$('.dMapContainer :checkbox').prop('checked', status);
			});
			$(document).on('click', '.btnCheckAllDMap', function(){
				$("#suppressDMap").prop('checked', false);
			});
			$(document).on('change', '.dMapContainer :checkbox', function(){
				$("#suppressDMap").prop('checked', false);
			});
			$('#colorSlider').slider({
				step: 1,
				min: 0,
				max: 100,
				values: [20, 40, 60, 80],
				slide: function (event, ui) {
					var colorGroup = ['Blue', 'Green', 'Yellow', 'Orange', 'Red'],
						colors = [0].concat(ui.values),
						result = [],
						min = 0;
					colors.push(100);
					for (var i = 0; i < colors.length - 1; i++) {
						if (min >= colors[i + 1]) {
							continue;
						}
						min = colors[i + 1];
						result.push(colorGroup[i] + ' (' + colors[i] + '% - ' + colors[i + 1] + '%) ');
					}
					$(this).parents('label').find('span').eq(0).text(result.join('  '));
				}
			});
			$(document).on('click', '#btnRestPenetration', function(){
				$('#colorSlider').slider('values', [20, 40, 60, 80]);
				return false;
			});
			this.haveInit = true;
		},
		onOpen: function () {
			this.init();
			console.log("option dialog openend");
			var dialog = $('#optionDialog');
			var option = dialog.data('option');
			$.each(option, function (k, v) {
				var ele = dialog.find('#' + k);
				if (ele.size() == 1 && ele.is('input') && ele.attr('type') == 'checkbox') {
					ele.prop("checked", v);
				} else if (ele.attr('type') == 'text') {
					ele.val(v);
				}
			});

			$('#optionDialog .range-slider').removeClass('disabled');
			$('#mapType-' + option.mapType).prop("checked", true);
			$('#optionDialog .dMapContainer input').prop("checked", false);
			if (option.distributorMap.length > 0) {
				$.each(option.distributorMap, function () {
					$('#optionDmap-' + this).prop("checked", true);
				});
			}
			$('#colorSlider').slider('values', option.PenetrationColors);
		},
		onApply: function () {
			var option = {},
				eleId;
			$('#optionDialog input[type=checkbox]').each(function () {
				eleId = $(this).attr('id');
				if (eleId.startsWith('optionDmap-')) {
					return true;
				}
				option[eleId] = $(this).prop("checked");
			});
			$('#optionDialog input[type=text]').each(function () {
				eleId = $(this).attr('id');
				option[eleId] = $(this).val();
			});
			var mapType = $('.mapTypeContainer input:checked').attr("id");
			if (mapType && mapType.startsWith('mapType-')) {
				option.mapType = mapType.substr(8);
			}
			var distributorMap = [];
			$('#optionDialog .dMapContainer input:checked').each(function () {
				distributorMap.push($(this).attr('id').replace('optionDmap-', ''));
			});
			option.distributorMap = distributorMap;
			option.PenetrationColors = $('#colorSlider').slider('values');
			return option;
		},
		onClose: function () {
			console.log("option dialog closed");
		}
	};
});