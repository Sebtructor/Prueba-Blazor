$('.digit-group').find('input').each(function () {
	$(this).on('keyup', function (e) {
		var parent = $($(this).parent());
		var numeroRegex = /^\d+$/;
		console.log($(this).val());
		if (e.keyCode === 8 || e.keyCode === 37) {
			var prev = parent.find('input#' + $(this).data('previous'));

			if (prev.length) {
				$(prev).select();
			}
		} else if (numeroRegex.test($(this).val())) {
			var next = parent.find('input#' + $(this).data('next'));

			if (next.length) {
				$(next).select();
			} else {
				if (parent.data('autosubmit')) {
					parent.submit();
				}
			}
		}
	});
});

function numberOnly(object) {
	if (object.value.length > 1)
		object.value = object.value.slice(0, 1)
}