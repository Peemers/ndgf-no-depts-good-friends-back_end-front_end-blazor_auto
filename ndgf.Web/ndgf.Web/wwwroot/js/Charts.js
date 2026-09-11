let monthlyChart = null;

window.renderMonthlyExpensesChart = (canvasId, labels, data) => {
    const ctx = document.getElementById(canvasId);

    if (monthlyChart) {
        monthlyChart.destroy();
    }

    monthlyChart = new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Dépenses par mois (€)',
                data: data,
                backgroundColor: '#1B3A3D'
            }]
        },
        options: {
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true
                }
            }
        }
    });
};