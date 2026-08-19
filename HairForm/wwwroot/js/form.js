// Константы
const hairPrice = 18;
const commision = 3;
// Цены аксессуаров
const accessoryPrices = {
    'small': 1,
    'medium': 3,
    'large': 8
};

// Элементы формы
const quantityInput = document.getElementById('quantity');
const needAccessoryCheckbox = document.getElementById('needAccessory');
const accessorySection = document.getElementById('accessorySection');
const totalPriceDisplay = document.getElementById('totalPriceDisplay');
const totalSumInput = document.getElementById('totalSumInput');
// Все чекбоксы аксессуаров и поля количества
const accessoryCheckboxes = document.querySelectorAll('.accessory-checkbox');
const accessoryQtyInputs = document.querySelectorAll('.accessory-qty');

// Получить суммарную стоимость выбранных аксессуаров
function getAccessoriesTotal() {
    let total = 0;
    accessoryCheckboxes.forEach((checkbox) => {
        if (checkbox.checked) {
            const targetId = checkbox.dataset.target; // qty_small и т.д.
            const qtyInput = document.getElementById(targetId);
            const qty = parseInt(qtyInput.value) || 0;
            const accessoryType = checkbox.id.replace('chk_', ''); // small, medium, large
            const price = accessoryPrices[accessoryType] || 0;
            total += price * qty;
        }
    });
    return total;
}

// Пересчёт итоговой суммы
function calculateTotal() {
    const quantity = parseInt(quantityInput.value) || 1; // если пусто, считаем 1
    const accessoriesTotal = needAccessoryCheckbox.checked ? getAccessoriesTotal() : 0;
    const total = quantity * hairPrice + accessoriesTotal + commision;

    totalPriceDisplay.textContent = total + '$';
    totalSumInput.value = total; // обновляем скрытое поле
}

// Показать/скрыть блок аксессуаров
function toggleAccessoryVisibility() {
    if (needAccessoryCheckbox.checked) {
        accessorySection.style.display = 'block';
    } else {
        accessorySection.style.display = 'none';
        // Сбрасываем все чекбоксы и количества
        accessoryCheckboxes.forEach((checkbox) => {
            checkbox.checked = false;
            const targetId = checkbox.dataset.target;
            document.getElementById(targetId).value = 0;
        });
    }
    calculateTotal();
}

// Обработчики событий
needAccessoryCheckbox.addEventListener('change', toggleAccessoryVisibility);

accessoryCheckboxes.forEach((checkbox) => {
    checkbox.addEventListener('change', () => {
        // Если чекбокс снят – обнуляем количество
        if (!checkbox.checked) {
            const targetId = checkbox.dataset.target;
            document.getElementById(targetId).value = 0;
        }
        calculateTotal();
    });
});

accessoryQtyInputs.forEach((input) => {
    input.addEventListener('input', calculateTotal);
});

quantityInput.addEventListener('input', calculateTotal);

function initDatepicker() {
    const datepickerInput = document.getElementById('datepicker');
    if (!datepickerInput) return; // если элемента нет на странице, выходим

    const bookedDatesUrl = datepickerInput.dataset.bookedDatesUrl;
    if (!bookedDatesUrl) {
        console.warn('Не указан URL для получения занятых дат');
        return;
    }

    fetch(bookedDatesUrl)
        .then(response => response.json())
        .then(bookedDates => {
            flatpickr(datepickerInput, {
                dateFormat: "Y-m-d",
                disable: bookedDates.map(d => new Date(d + "T00:00:00")),
                minDate: "today",
                locale: "ru"
            });
        })
        .catch(error => console.error('Ошибка загрузки занятых дат:', error));
}

// Инициализация при загрузке
window.addEventListener('DOMContentLoaded', () => {
    toggleAccessoryVisibility();
    initDatepicker();
    const errorMessageElement = document.getElementById('errorMessage');
    if (errorMessageElement && errorMessageElement.value) {
        const errorModal = new bootstrap.Modal(document.getElementById('errorModal'));
        errorModal.show();
    }
});