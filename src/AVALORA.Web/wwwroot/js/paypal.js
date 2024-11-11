$(function () {
	paypal.Buttons({
		async createOrder() {
			const response = await fetch('/Payment/Create', {
				method: "POST",
				headers: {
					"Content-Type": "application/json",
				},
				body: JSON.stringify({
					amount: document.getElementById('totalAmount').value
				})
			});

			const order = await response.json();

			return order.id;
		},

		async onApprove(data) {
			// Capture the funds from the transaction.
			const response = await fetch('/Payment/Complete', {
				method: "POST",
				headers: {
					"Content-Type": "application/json",
				},
				body: JSON.stringify({
					orderID: data.orderID
				})
			})

			const details = await response.json();

			// Show success message to buyer
			if (details.success) {
				window.location.href = details.url;
			}
			else {
				document.getElementById("notification-container").innerHTML = `
									<div class="alert alert-danger alert-dismissible fade show" role="alert">
										<strong>Transaction Error!</strong>
										<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
									</div>`
			}
		},

		onCancel(data) {
			document.getElementById("notification-container").innerHTML = `
							<div class="alert alert-danger alert-dismissible fade show" role="alert">
								<strong>Payment Canceled!</strong>
								<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
							</div>`
		},
		onError(err) {
			document.getElementById("notification-container").innerHTML = `
							<div class="alert alert-danger alert-dismissible fade show" role="alert">
								<strong>An error occurred!</strong>
								<button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
							</div>`
		}
	}).render('#paypal-button-container');
})