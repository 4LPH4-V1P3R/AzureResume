window.addEventListener('DOMContentLoaded', (event) => {
    getVisitCount();

    const categoryTitles = document.querySelectorAll('.category-title');
    categoryTitles.forEach(function(title) {
        title.addEventListener('click', function() {
            console.log('Clicked!');
            const category = this.parentElement;
            category.classList.toggle('open');
        });
    });
});

const functionApi = 'https://khardiman-resume-api.azurewebsites.net/api/GetResumeCounter';

const getVisitCount = () => {
    let count = 30;
    fetch(functionApi).then(response => {
        return response.json()
    }).then(response => {
        console.log("Website called function API.");
        count = response.count;
        document.getElementById("counter").innerText = count;
    }).catch(function(error) {
        console.log(error);
        document.getElementById("counter").innerText = count;
    });
    return count;
}
