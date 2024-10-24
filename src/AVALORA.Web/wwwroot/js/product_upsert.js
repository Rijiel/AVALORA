const quill = new Quill('#description_input', {
	modules: {
		'toolbar': [
			[{ 'font': [] }, { 'size': [] }],
			['bold', 'italic', 'underline', 'strike'],
			[{ 'color': [] }, { 'background': [] }],
			[{ 'header': '1' }, { 'header': '2' }, 'blockquote', 'code-block'],
			[{ 'list': 'ordered' }, { 'list': 'bullet' }, { 'indent': '-1' }, { 'indent': '+1' }],
			['link', 'image']
		]
	},
	theme: 'snow',
	placeholder: 'Type your description here...'
});

function handleSubmit() {
	document.getElementById('Description').value = quill.root.innerHTML;
};