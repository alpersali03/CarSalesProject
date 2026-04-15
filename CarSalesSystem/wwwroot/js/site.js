(() => {
    const storageKey = "car-compare-selection";
    const maxCars = 4;

    const safeParse = (value) => {
        try {
            const parsed = JSON.parse(value ?? "[]");
            return Array.isArray(parsed) ? parsed.map(Number).filter(Number.isInteger) : [];
        } catch {
            return [];
        }
    };

    const getIds = () => safeParse(window.localStorage.getItem(storageKey));

    const setIds = (ids) => {
        window.localStorage.setItem(storageKey, JSON.stringify(ids.slice(0, maxCars)));
    };

    const updateUi = () => {
        const ids = getIds();
        const dock = document.querySelector("[data-compare-dock]");
        const summary = document.querySelector("[data-compare-summary]");
        const countBadge = document.querySelector("[data-compare-count]");
        const links = document.querySelectorAll("[data-compare-link]");

        if (countBadge) {
            countBadge.textContent = ids.length.toString();
        }

        links.forEach((link) => {
            const url = new URL(link.href || `${window.location.origin}/Car/Compare`, window.location.origin);
            url.searchParams.set("ids", ids.join(","));
            link.href = url.pathname + url.search;
        });

        document.querySelectorAll("[data-compare-button]").forEach((button) => {
            const carId = Number(button.getAttribute("data-car-id"));
            const selected = ids.includes(carId);
            button.textContent = selected ? "Remove compare" : "Compare";
            button.classList.toggle("btn-dark", selected);
            button.classList.toggle("btn-outline-secondary", !selected);
        });

        if (dock && summary) {
            dock.hidden = ids.length === 0;
            summary.textContent = ids.length === 0
                ? "Select up to 4 cars to compare them side by side."
                : `${ids.length} car${ids.length > 1 ? "s" : ""} selected for compare.`;
        }
    };

    document.addEventListener("click", (event) => {
        const compareButton = event.target.closest("[data-compare-button]");
        if (compareButton) {
            const carId = Number(compareButton.getAttribute("data-car-id"));
            const ids = getIds();
            const isSelected = ids.includes(carId);

            if (isSelected) {
                setIds(ids.filter((id) => id !== carId));
            } else if (ids.length < maxCars) {
                setIds([...ids, carId]);
            }

            updateUi();
        }

        const clearButton = event.target.closest("[data-compare-clear]");
        if (clearButton) {
            setIds([]);
            updateUi();
        }
    });

    updateUi();
})();
