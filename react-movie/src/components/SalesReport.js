import React, { useState, useEffect } from 'react';
import { getSalesReport, exportSalesReportToExcel } from '../services/apiService';
import './css/SalesReport.css';

function SalesReport() {
    const [salesData, setSalesData] = useState({ movieSales: [], itemSales: [] });
    const [error, setError] = useState(null);

    useEffect(() => {
        fetchSalesData();
    }, []);

    // Lấy dữ liệu báo cáo doanh thu từ API
    const fetchSalesData = async () => {
        try {
            const response = await getSalesReport();
            setSalesData(response.data);
        } catch (error) {
            console.error('Error fetching sales data:', error);
            setError('Failed to fetch sales data');
        }
    };

    // Xuất báo cáo doanh thu ra file Excel
    const exportToExcel = async () => {
        try {
            const response = await exportSalesReportToExcel();
            const url = window.URL.createObjectURL(new Blob([response.data]));
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', 'SalesReport.xlsx');
            document.body.appendChild(link);
            link.click();
        } catch (error) {
            console.error('Error exporting to Excel:', error);
            setError('Failed to export to Excel');
        }
    };

    return (
        <div className="sales-report">
            <h2>Báo cáo doanh thu</h2>
            {error && <p className="error-message">{error}</p>}
            <button onClick={exportToExcel} className="export-button">Xuất ra Excel</button>

            {/* Bảng báo cáo doanh thu phim */}
            <h3>Doanh thu phim</h3>
            <table className="sales-table">
                <thead>
                    <tr>
                        <th>Tên phim</th>
                        <th>Số vé bán</th>
                        <th>Tổng doanh thu</th>
                    </tr>
                </thead>
                <tbody>
                    {salesData.movieSales && salesData.movieSales.map((data, index) => (
                        <tr key={index}>
                            <td>{data.movieTitle}</td>
                            <td>{data.ticketsSold}</td>
                            <td>{data.totalRevenue} VND</td>
                        </tr>
                    ))}
                </tbody>
            </table>

            {/* Bảng báo cáo doanh thu items */}
            <h3>Doanh thu món ăn và đồ uống</h3>
            <table className="sales-table">
                <thead>
                    <tr>
                        <th>Tên món</th>
                        <th>Số lượng bán</th>
                        <th>Tổng doanh thu</th>
                    </tr>
                </thead>
                <tbody>
                    {salesData.itemSales && salesData.itemSales.map((data, index) => (
                        <tr key={index}>
                            <td>{data.itemName}</td>
                            <td>{data.quantitySold}</td>
                            <td>{data.totalRevenue} VND</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default SalesReport;


