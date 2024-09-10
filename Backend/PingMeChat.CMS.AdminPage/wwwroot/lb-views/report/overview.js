

$(document).ready(function () {
    // Dữ liệu mẫu cho biểu đồ
    const sampleChartData = {
        today: {
            labels: ['00:00', '02:00', '04:00', '06:00', '08:00', '10:00', '12:00', '14:00', '16:00', '18:00', '20:00', '22:00'],
            costs: [100000, 150000, 200000, 180000, 220000, 250000, 300000, 280000, 260000, 220000, 180000, 150000],
            revenues: [150000, 200000, 250000, 300000, 350000, 400000, 450000, 500000, 480000, 420000, 380000, 300000]
        },
        yesterday: {
            labels: ['00:00', '02:00', '04:00', '06:00', '08:00', '10:00', '12:00', '14:00', '16:00', '18:00', '20:00', '22:00'],
            costs: [90000, 140000, 190000, 170000, 210000, 240000, 290000, 270000, 250000, 210000, 170000, 140000],
            revenues: [140000, 190000, 240000, 290000, 340000, 390000, 440000, 490000, 470000, 410000, 370000, 290000]
        },
        thisWeek: {
            labels: ['Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7', 'Chủ nhật'],
            costs: [1000000, 1200000, 1100000, 1300000, 1400000, 1600000, 1500000],
            revenues: [1500000, 1800000, 1700000, 2000000, 2200000, 2500000, 2300000]
        },
        thisMonth: {
            labels: [...Array(30).keys()].map(i => `Ngày ${i + 1}`),
            costs: Array(30).fill().map(() => Math.floor(Math.random() * 500000) + 1000000),
            revenues: Array(30).fill().map(() => Math.floor(Math.random() * 1000000) + 1500000)
        }
    };

    let chart;

    function initChart() {
        const ctx = document.getElementById('revenueChart').getContext('2d');
        chart = new Chart(ctx, {
            type: 'line',
            data: {
                labels: [],
                datasets: [{
                    label: 'Chi phí',
                    data: [],
                    borderColor: 'red',
                    fill: false
                }, {
                    label: 'Doanh thu',
                    data: [],
                    borderColor: 'blue',
                    fill: false
                }]
            },
            options: {
                responsive: true,
                maintainAspectRatio: false,
                scales: {
                    y: {
                        beginAtZero: true
                    }
                }
            }
        });
    }

    function updateChart() {
        const timeRange = document.getElementById('timeRange').value;
        const customDateRange = document.getElementById('customDateRange');

        if (timeRange === 'custom') {
            customDateRange.style.display = 'block';
            // Xử lý ngày tùy chỉnh ở đây nếu cần
        } else {
            customDateRange.style.display = 'none';
            const data = sampleChartData[timeRange];

            chart.data.labels = data.labels;
            chart.data.datasets[0].data = data.costs;
            chart.data.datasets[1].data = data.revenues;
            chart.update();
        }
    }

    // Hàm để cập nhật tổng quan
    function updateSummary() {
        const totalCost = sampleData.dailyReport.reduce((sum, day) => sum + day.revenue - day.profit, 0);
        const totalOrders = sampleData.dailyReport.reduce((sum, day) => sum + day.customers, 0);
        const totalRevenue = sampleData.dailyReport.reduce((sum, day) => sum + day.revenue, 0);
        const totalProfit = sampleData.dailyReport.reduce((sum, day) => sum + day.profit, 0);

        document.getElementById('totalCost').textContent = totalCost.toLocaleString() + ' đ';
        document.getElementById('totalOrders').textContent = totalOrders;
        document.getElementById('totalRevenue').textContent = totalRevenue.toLocaleString() + ' đ';
        document.getElementById('totalProfit').textContent = totalProfit.toLocaleString() + ' đ';
    }

    // Hàm để tạo báo cáo theo ngày
    function generateReport() {
        const reportDate = document.getElementById('reportDate').value;
        const tableBody = document.querySelector('#dailyReportTable tbody');
        tableBody.innerHTML = '';

        const reportData = sampleData.dailyReport.filter(day => day.date === reportDate);

        if (reportData.length > 0) {
            reportData.forEach(day => {
                const row = `<tr>
                                        <td>${day.date}</td>
                                        <td>${day.revenue.toLocaleString()} đ</td>
                                        <td>${day.refund.toLocaleString()} đ</td>
                                        <td>${day.discount.toLocaleString()} đ</td>
                                        <td>${day.netRevenue.toLocaleString()} đ</td>
                                        <td>${day.customers}</td>
                                        <td>${day.profit.toLocaleString()} đ</td>
                                        <td>${day.rate}</td>
                                    </tr>`;
                tableBody.innerHTML += row;
            });
        } else {
            tableBody.innerHTML = '<tr><td colspan="8">Không có dữ liệu cho ngày này</td></tr>';
        }
    }

    // Hàm để tìm kiếm sản phẩm
    function searchProducts() {
        const category = document.getElementById('productCategory').value;
        const date = document.getElementById('productDate').value;
        const searchTerm = document.getElementById('productSearch').value.toLowerCase();

        const filteredProducts = sampleData.products.filter(product =>
            (category === 'all' || product.category === category) &&
            product.name.toLowerCase().includes(searchTerm)
        );

        const tableBody = document.querySelector('#productTable tbody');
        tableBody.innerHTML = '';

        if (filteredProducts.length > 0) {
            filteredProducts.forEach(product => {
                const row = `<tr>
                                        <td>${product.name}</td>
                                        <td>${product.quantity}</td>
                                        <td>${product.revenue.toLocaleString()} đ</td>
                                        <td>${product.profit.toLocaleString()} đ</td>
                                    </tr>`;
                tableBody.innerHTML += row;
            });
        } else {
            tableBody.innerHTML = '<tr><td colspan="4">Không tìm thấy sản phẩm phù hợp</td></tr>';
        }
    }

    // Hàm để xuất báo cáo (giả lập)
    function exportReport() {
        alert('Báo cáo đã được xuất thành công!');
    }

    // Biểu đồ doanh thu
    //var ctx = document.getElementById('revenueChart').getContext('2d');
    //var chart = new Chart(ctx, {
    //    type: 'line',
    //    data: {
    //        labels: ['00:00', '02:00', '04:00', '06:00', '08:00', '10:00', '12:00', '14:00', '16:00', '18:00', '20:00', '22:00'],
    //        datasets: [{
    //            label: 'Chi phí',
    //            data: [100000, 150000, 200000, 180000, 220000, 250000, 300000, 280000, 260000, 220000, 180000, 150000],
    //            borderColor: 'red',
    //            fill: false
    //        }, {
    //            label: 'Doanh thu',
    //            data: [150000, 200000, 250000, 300000, 350000, 400000, 450000, 500000, 480000, 420000, 380000, 300000],
    //            borderColor: 'blue',
    //            fill: false
    //        }]
    //    },
    //    options: {
    //        responsive: true,
    //        maintainAspectRatio: false,
    //        scales: {
    //            y: {
    //                beginAtZero: true
    //            }
    //        }
    //    }
    //});

    // Khởi tạo trang
    initChart();
    updateChart();

    updateSummary();
    generateReport();
    searchProducts();
});