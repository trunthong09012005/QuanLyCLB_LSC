// Reports Redesigned - Main JavaScript

let chartsInstance = {
    membersChart: null,
    activitiesChart: null,
    projectsChart: null,
    financeChart: null
};

document.addEventListener('DOMContentLoaded', function () {
    try {
        initializeCharts();
    } catch (error) {
        console.error('Error initializing charts:', error);
    }
});

function initializeCharts() {
    const container = document.getElementById('reportData');
    if (!container) {
        console.warn('reportData container not found');
        return;
    }

    function toNumberArray(arr) {
        return (arr || []).map(v => {
            if (v === null || v === undefined || v === '') return 0;
            const n = Number(v);
            return isNaN(n) ? 0 : n;
        });
    }

    function parseAttr(name) {
        try {
            return JSON.parse(container.getAttribute(name) || '[]');
        } catch (e) {
            console.warn('Invalid JSON for', name, e);
            return [];
        }
    }

    // Read data from attributes
    let membersLabels = parseAttr('data-members-labels');
    let membersData = toNumberArray(parseAttr('data-members-data'));
    let activitiesLabels = parseAttr('data-activities-labels');
    let activitiesData = toNumberArray(parseAttr('data-activities-data'));
    let projectsLabels = parseAttr('data-projects-labels');
    let projectsData = toNumberArray(parseAttr('data-projects-data'));
    let financeLabels = parseAttr('data-finance-labels');
    let financeIncome = toNumberArray(parseAttr('data-finance-income'));
    let financeExpense = toNumberArray(parseAttr('data-finance-expense'));

    const isEmptySeries = arr => !arr || arr.length === 0 || arr.every(v => v === 0);

    if (isEmptySeries(membersData) && isEmptySeries(activitiesData) && isEmptySeries(projectsData) && 
        (isEmptySeries(financeIncome) && isEmptySeries(financeExpense))) {
        // Optionally fetch from API if all data is empty
        console.log('No chart data available');
    }

    renderCharts(membersLabels, membersData, activitiesLabels, activitiesData,
                 projectsLabels, projectsData, financeLabels, financeIncome, financeExpense);
}

function renderCharts(membersLabels, membersData, activitiesLabels, activitiesData,
                      projectsLabels, projectsData, financeLabels, financeIncome, financeExpense) {
    
    // Colors configuration
    const colors = {
        primary: 'rgba(91, 109, 230, 1)',
        primaryLight: 'rgba(91, 109, 230, 0.6)',
        primaryVeryLight: 'rgba(91, 109, 230, 0.1)',
        secondary: 'rgba(123, 143, 243, 1)',
        success: 'rgba(46, 204, 113, 1)',
        successLight: 'rgba(46, 204, 113, 0.15)',
        danger: 'rgba(231, 76, 60, 1)',
        dangerLight: 'rgba(231, 76, 60, 0.12)',
        info: 'rgba(52, 152, 219, 1)',
        warning: 'rgba(241, 196, 15, 1)',
        orange: 'rgba(243, 156, 18, 1)'
    };

    // Chart options
    const commonOptions = {
        responsive: true,
        maintainAspectRatio: true,
        plugins: {
            legend: {
                display: true,
                position: 'bottom',
                labels: {
                    padding: 15,
                    font: { size: 12, weight: '500' },
                    usePointStyle: true
                }
            },
            tooltip: {
                backgroundColor: 'rgba(0, 0, 0, 0.8)',
                padding: 12,
                titleFont: { size: 13, weight: '600' },
                bodyFont: { size: 12 },
                cornerRadius: 6,
                displayColors: true
            }
        },
        scales: {
            y: {
                beginAtZero: true,
                ticks: {
                    font: { size: 11 },
                    color: '#666'
                },
                grid: {
                    color: 'rgba(0, 0, 0, 0.05)',
                    drawBorder: false
                }
            },
            x: {
                ticks: {
                    font: { size: 11 },
                    color: '#666'
                },
                grid: {
                    display: false,
                    drawBorder: false
                }
            }
        }
    };

    // 1. Members Chart
    const membersCanvas = document.getElementById('membersChart');
    if (membersCanvas) {
        const ctx = membersCanvas.getContext('2d');
        const gradient = ctx.createLinearGradient(0, 0, 0, 300);
        gradient.addColorStop(0, colors.primaryLight);
        gradient.addColorStop(1, colors.primaryVeryLight);

        if (chartsInstance.membersChart) {
            chartsInstance.membersChart.destroy();
        }

        chartsInstance.membersChart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: membersLabels,
                datasets: [{
                    label: 'Thành viên',
                    data: membersData,
                    backgroundColor: gradient,
                    borderColor: colors.primary,
                    borderWidth: 3,
                    fill: true,
                    tension: 0.4,
                    pointRadius: 5,
                    pointHoverRadius: 7,
                    pointBackgroundColor: colors.primary,
                    pointBorderColor: '#fff',
                    pointBorderWidth: 2
                }]
            },
            options: {
                ...commonOptions,
                plugins: {
                    ...commonOptions.plugins,
                    legend: { display: false }
                }
            }
        });

        if (membersData.every(v => v === 0)) {
            // remove existing empty-state if present
            const existing = membersCanvas.parentElement.parentElement.querySelector('.empty-state');
            if (existing) existing.remove();
            membersCanvas.parentElement.insertAdjacentHTML('afterend',
                '<div class="empty-state"><p>Chưa có dữ liệu</p></div>');
        }
    }

    // 2. Activities Chart
    const activitiesCanvas = document.getElementById('activitiesChart');
    if (activitiesCanvas) {
        const ctx = activitiesCanvas.getContext('2d');

        if (chartsInstance.activitiesChart) {
            chartsInstance.activitiesChart.destroy();
        }

        chartsInstance.activitiesChart = new Chart(ctx, {
            type: 'bar',
            data: {
                labels: activitiesLabels,
                datasets: [{
                    label: 'Ho?t ??ng',
                    data: activitiesData,
                    backgroundColor: colors.success,
                    borderColor: colors.success,
                    borderRadius: 6,
                    borderSkipped: false,
                    hoverBackgroundColor: colors.dangerLight
                }]
            },
            options: {
                ...commonOptions,
                plugins: {
                    ...commonOptions.plugins,
                    legend: { display: false }
                }
            }
        });

        if (activitiesData.every(v => v === 0)) {
            const existing = activitiesCanvas.parentElement.parentElement.querySelector('.empty-state');
            if (existing) existing.remove();
            activitiesCanvas.parentElement.insertAdjacentHTML('afterend',
                '<div class="empty-state"><p>Chưa có dữ liệu</p></div>');
        }
    }

    // 3. Projects Chart
    const projectsCanvas = document.getElementById('projectsChart');
    if (projectsCanvas) {
        const ctx = projectsCanvas.getContext('2d');

        if (chartsInstance.projectsChart) {
            chartsInstance.projectsChart.destroy();
        }

        const bgColors = [colors.primary, colors.secondary, colors.orange, colors.warning, colors.danger];

        chartsInstance.projectsChart = new Chart(ctx, {
            type: 'doughnut',
            data: {
                labels: projectsLabels,
                datasets: [{
                    data: projectsData,
                    backgroundColor: bgColors.slice(0, projectsLabels.length),
                    borderColor: '#fff',
                    borderWidth: 2,
                    hoverOffset: 10
                }]
            },
            options: {
                ...commonOptions,
                scales: undefined, // Pie/Doughnut charts don't use scales
                plugins: {
                    ...commonOptions.plugins,
                    legend: {
                        display: true,
                        position: 'right',
                        labels: {
                            padding: 15,
                            font: { size: 12, weight: '500' },
                            usePointStyle: true
                        }
                    }
                }
            }
        });

        if (projectsData.every(v => v === 0)) {
            const existing = projectsCanvas.parentElement.parentElement.querySelector('.empty-state');
            if (existing) existing.remove();
            projectsCanvas.parentElement.insertAdjacentHTML('afterend',
                '<div class="empty-state"><p>Chưa có dữ liệu</p></div>');
        }
    }

    // 4. Finance Chart
    const financeCanvas = document.getElementById('financeChart');
    if (financeCanvas) {
        const ctx = financeCanvas.getContext('2d');

        if (chartsInstance.financeChart) {
            chartsInstance.financeChart.destroy();
        }

        chartsInstance.financeChart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: financeLabels,
                datasets: [
                    {
                        label: 'Thu',
                        data: financeIncome,
                        borderColor: colors.success,
                        backgroundColor: colors.successLight,
                        borderWidth: 3,
                        fill: true,
                        tension: 0.4,
                        pointRadius: 4,
                        pointHoverRadius: 6,
                        pointBackgroundColor: colors.success,
                        pointBorderColor: '#fff',
                        pointBorderWidth: 2
                    },
                    {
                        label: 'Chi',
                        data: financeExpense,
                        borderColor: colors.danger,
                        backgroundColor: colors.dangerLight,
                        borderWidth: 3,
                        fill: true,
                        tension: 0.4,
                        pointRadius: 4,
                        pointHoverRadius: 6,
                        pointBackgroundColor: colors.danger,
                        pointBorderColor: '#fff',
                        pointBorderWidth: 2
                    }
                ]
            },
            options: commonOptions
        });

        if (financeIncome.every(v => v === 0) && financeExpense.every(v => v === 0)) {
            const existing = financeCanvas.parentElement.parentElement.querySelector('.empty-state');
            if (existing) existing.remove();
            financeCanvas.parentElement.insertAdjacentHTML('afterend',
                '<div class="empty-state"><p>Chưa có dữ liệu</p></div>');
        }
    }
}

function filterCharts() {
    const yearFilter = document.getElementById('yearFilter');
    if (yearFilter) {
        const year = yearFilter.value;
        window.location.href = `?year=${year}`;
    }
}

function viewCategory() {
    const categoryFilter = document.getElementById('categoryFilter');
    if (categoryFilter && categoryFilter.value) {
        const category = categoryFilter.value;
        window.location.href = `/Reports/CategoryDetails?category=${category}`;
    }
}

function applyFilters() {
    // This is already handled by the form submission
    // but you can add additional logic here if needed
    filterCharts();
}
