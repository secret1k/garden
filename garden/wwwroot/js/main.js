async function getCategories() {
    const response = await fetch("/api/categories", {
        method: "GET",
        headers: { "Accept": "application/json" }
    });
    if (response.ok === true) {
        const categories = await response.json();
        const categoryList = document.getElementById("category-list");
        console.log(categories);
        for (category of categories) {
            categoryList.append(createCard(category.name, category.img));
        }
    }
};

function createCard(name, img) {
    let card = document.createElement("div");
    let image = document.createElement("img");
    let divOverlay = document.createElement("div");
    let h5 = document.createElement("h5");

    card.classList.add("card", "col");
    image.classList.add("card-img");
    image.src = img;
    divOverlay.classList.add("card-img-overlay");
    h5.classList.add("card-title");
    h5.innerText = name;

    divOverlay.appendChild(h5);
    card.append(image, divOverlay);

    return card;
};

getCategories();