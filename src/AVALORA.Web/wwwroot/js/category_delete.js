function Delete(url) {
	const token = $('input[name="__RequestVerificationToken"]').val();

	Swal.fire({
		title: "Are you sure?",
		text: "You won't be able to revert this!",
		icon: "warning",
		showCancelButton: true,
		confirmButtonColor: "#3085d6",
		cancelButtonColor: "#d33",
		confirmButtonText: "Yes, delete it!"
	}).then((result) => {
		if (result.isConfirmed) {			
			$.ajax({
				url: url,
				type: 'DELETE',
				headers: {
					'RequestVerificationToken': token
				},
				success: function (data) {
					if (data.success) {
						window.location.href = data.redirectUrl;
					}
					else {
						location.reload(true);
					}
				}
			})
		}
	});
}