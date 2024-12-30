import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './css/TheaterManager.css';

function TheaterManager() {
    const [theaters, setTheaters] = useState([]);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editTheater, setEditTheater] = useState(null);
    const [formData, setFormData] = useState({ name: '', location: '', rows: 0, columns: 0 });
    const [error, setError] = useState(null);

    useEffect(() => {
        fetchTheaters();
    }, []);

    // Lấy danh sách phòng chiếu từ API
    const fetchTheaters = async () => {
        try {
            const response = await axios.get('http://localhost:5175/api/Theater');
            setTheaters(response.data);
        } catch (error) {
            console.error('Error fetching theaters:', error);
            setError('Failed to fetch theaters');
        }
    };

    // Mở modal thêm hoặc chỉnh sửa
    const openModal = (theater = null) => {
        setIsModalOpen(true);
        if (theater) {
            setEditTheater(theater);
            setFormData({ name: theater.name, location: theater.location, rows: theater.rows, columns: theater.columns });
        } else {
            setEditTheater(null);
            setFormData({ name: '', location: '', rows: 0, columns: 0 });
        }
    };

    // Đóng modal
    const closeModal = () => {
        setIsModalOpen(false);
        setEditTheater(null);
        setFormData({ name: '', location: '', rows: 0, columns: 0 });
    };

    // Xử lý thay đổi đầu vào
    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
    };

    // Xử lý thêm hoặc cập nhật phòng chiếu
    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            if (editTheater) {
                // Cập nhật phòng chiếu
                await axios.put(`http://localhost:5175/api/Theater/${editTheater.theaterId}`, formData);
            } else {
                // Thêm phòng chiếu mới
                await axios.post('http://localhost:5175/api/Theater', formData);
            }
            fetchTheaters();
            closeModal();
        } catch (error) {
            console.error('Error saving theater:', error);
            setError('Failed to save theater');
        }
    };

    // Xóa phòng chiếu
    const handleDelete = async (theaterId) => {
        if (window.confirm('Are you sure you want to delete this theater?')) {
            try {
                await axios.delete(`http://localhost:5175/api/Theater/${theaterId}`);
                fetchTheaters();
            } catch (error) {
                console.error('Error deleting theater:', error);
                setError('Failed to delete theater');
            }
        }
    };

    return (
        <div className="theater-manager">
            <h2>Quản lý phòng chiếu</h2>
            {error && <p className="error-message">{error}</p>}
            <button onClick={() => openModal()} className="add-theater-button">Thêm phòng chiếu</button>

            {/* Danh sách các phòng chiếu */}
            <div className="theater-list">
                {theaters.map(theater => (
                    <div key={theater.theaterId} className="theater-card">
                        <p><strong>Tên phòng chiếu:</strong> {theater.name}</p>
                        <p><strong>Địa điểm:</strong> {theater.location}</p>
                        <p><strong>Số hàng:</strong> {theater.rows}</p>
                        <p><strong>Số cột:</strong> {theater.columns}</p>
                        <button onClick={() => openModal(theater)} className="edit-button">Chỉnh sửa</button>
                        <button onClick={() => handleDelete(theater.theaterId)} className="delete-button">Xóa</button>
                    </div>
                ))}
            </div>

            {/* Modal để thêm hoặc chỉnh sửa phòng chiếu */}
            {isModalOpen && (
                <div className="modal">
                    <div className="modal-content">
                        <h3>{editTheater ? 'Chỉnh sửa phòng chiếu' : 'Thêm phòng chiếu'}</h3>
                        <form onSubmit={handleSubmit}>
                            <label>
                                Tên phòng chiếu:
                                <input
                                    type="text"
                                    name="name"
                                    value={formData.name}
                                    onChange={handleChange}
                                    required
                                />
                            </label>
                            <label>
                                Địa điểm:
                                <input
                                    type="text"
                                    name="location"
                                    value={formData.location}
                                    onChange={handleChange}
                                    required
                                />
                            </label>
                            <label>
                                Số hàng:
                                <input
                                    type="number"
                                    name="rows"
                                    value={formData.rows}
                                    onChange={handleChange}
                                    required
                                />
                            </label>
                            <label>
                                Số cột:
                                <input
                                    type="number"
                                    name="columns"
                                    value={formData.columns}
                                    onChange={handleChange}
                                    required
                                />
                            </label>
                            <div className="modal-buttons">
                                <button type="submit" className="save-button">{editTheater ? 'Cập nhật' : 'Thêm'}</button>
                                <button type="button" onClick={closeModal} className="cancel-button">Hủy</button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
}

export default TheaterManager;

