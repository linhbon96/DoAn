import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './css/SalesReport.css';

function SalesReport() {
    const [salesData, setSalesData] = useState([]);
    const [error, setError] = useState(null);

    useEffect(() => {
        fetchSalesData();
    }, []);

    // Lấy dữ liệu báo cáo doanh thu từ API
    const fetchSalesData = async () => {
        try {
            const response = await axios.get('http://localhost:5175/api/Report/SalesReport');
            setSalesData(response.data);
        } catch (error) {
            console.error('Error fetching sales data:', error);
            setError('Failed to fetch sales data');
        }
    };

    // Xuất báo cáo doanh thu ra file Excel
    const exportToExcel = async () => {
        try {
            const response = await axios.get('http://localhost:5175/api/Report/ExportToExcel', {
                responseType: 'blob',
            });
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

            {/* Bảng báo cáo doanh thu */}
            <table className="sales-table">
                <thead>
                    <tr>
                        <th>Tên phim</th>
                        <th>Số vé bán</th>
                        <th>Tổng doanh thu</th>
                    </tr>
                </thead>
                <tbody>
                    {salesData.map((data, index) => (
                        <tr key={index}>
                            <td>{data.movieTitle}</td>
                            <td>{data.ticketsSold}</td>
                            <td>{data.totalRevenue} VND</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}

export default SalesReport;

