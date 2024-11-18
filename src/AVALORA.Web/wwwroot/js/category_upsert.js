function capitalizeFirstLetter(input) {
    const value = input.value;
    if (value.length > 0) {
        input.value = value.charAt(0).toUpperCase() + value.slice(1);
    }
}