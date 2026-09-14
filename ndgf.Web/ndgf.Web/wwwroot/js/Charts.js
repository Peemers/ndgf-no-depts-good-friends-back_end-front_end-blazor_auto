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

let expensesByMemberChart = null;

window.renderExpensesByMemberChart = (canvasId, labels, data) => {
    const ctx = document.getElementById(canvasId);

    if (expensesByMemberChart) {
        expensesByMemberChart.destroy();
    }

    expensesByMemberChart = new Chart(ctx, {
        type: 'pie',
        data: {
            labels: labels,
            datasets: [{
                label: 'répartition par membre',
                data: data,
                backgroundColor: ['#1B3A3D', '#6B3320', '#5E4213', '#1E1813', '#3D5A45']
            }]
        },
        options: {
            responsive: true,
            plugins: {
                title: {
                    display: true,
                    text: 'Répartition des dépenses par membre'
                },
                tooltip: {
                    callbacks: {
                        label: function (context) {
                            return context.label + ' : ' + context.parsed + ' €';
                        }
                    }
                }
            }
        }
    });
}