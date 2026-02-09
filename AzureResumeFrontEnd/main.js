window.addEventListener('DOMContentLoaded', () => {
    getVisitCount();

    const categoryTitles = document.querySelectorAll('.category-title');
    categoryTitles.forEach(function(title) {
        title.addEventListener('click', function() {
            const category = this.parentElement;
            category.classList.toggle('open');
        });
    });
});

const functionApi = 'https://khardiman-resume-api.azurewebsites.net/api/GetResumeCounter';

const getVisitCount = async () => {
    const counterEl = document.getElementById("counter");
    counterEl.innerText = "Loading...";
    counterEl.className = "loading";

    try {
        const response = await fetch(functionApi);
        const data = await response.json();
        console.log("Website called function API.");
        counterEl.innerText = data.count;
        counterEl.className = "";
    } catch (error) {
        console.error("Failed to fetch visitor count:", error);
        counterEl.innerText = "unavailable";
        counterEl.className = "error";
    }
};
