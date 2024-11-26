import React, { useEffect, useState } from 'react';
import axios from 'axios';
import './css/Item.css';

function Item() {
    const [items, setItems] = useState([]);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editItem, setEditItem] = useState(null); // Đối tượng đang chỉnh sửa hoặc thêm mới
    const [formData, setFormData] = useState({ name: '', price: '' });
    const [error, setError] = useState(null);

    useEffect(() => {
        fetchItems();
    }, []);

    // Lấy danh sách vật phẩm từ API
    const fetchItems = async () => {
        try {
            const response = await axios.get('http://localhost:5175/api/Item');
            setItems(response.data);
        } catch (error) {
            console.error('Error fetching items:', error);
            setError('Failed to fetch items');
        }
    };

    // Mở modal thêm hoặc chỉnh sửa
    const openModal = (item = null) => {
        setIsModalOpen(true);
        if (item) {
            setEditItem(item);
            setFormData({ name: item.name, price: item.price });
        } else {
            setEditItem(null);
            setFormData({ name: '', price: '' });
        }
    };

    // Đóng modal
    const closeModal = () => {
        setIsModalOpen(false);
        setEditItem(null);
        setFormData({ name: '', price: '' });
    };

    // Xử lý thay đổi đầu vào
    const handleChange = (e) => {
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
    };

    // Xử lý thêm hoặc cập nhật vật phẩm
    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            if (editItem) {
                // Cập nhật vật phẩm
                await axios.put(`http://localhost:5175/api/Item/${editItem.itemId}`, formData);
            } else {
                // Thêm vật phẩm mới
                await axios.post('http://localhost:5175/api/Item', formData);
            }
            fetchItems();
            closeModal();
        } catch (error) {
            console.error('Error saving item:', error);
            setError('Failed to save item');
        }
    };

    // Xóa vật phẩm
    const handleDelete = async (itemId) => {
        if (window.confirm('Are you sure you want to delete this item?')) {
            try {
                await axios.delete(`http://localhost:5175/api/Item/${itemId}`);
                fetchItems();
            } catch (error) {
                console.error('Error deleting item:', error);
                setError('Failed to delete item');
            }
        }
    };

    return (
        <div className="item-manager">
            <h2>Quản lý món</h2>
            {error && <p className="error-message">{error}</p>}
            <button onClick={() => openModal()} className="add-item-button">Thêm món</button>
            
            {/* Danh sách các vật phẩm */}
            <div className="item-list">
                {items.map(item => (
                    <div key={item.itemId} className="item-card">
                        <p><strong>Tên món:</strong> {item.name}</p>
                        <p><strong>Giá:</strong> {item.price} VND</p>
                        <button onClick={() => openModal(item)} className="edit-button">Chỉnh sửa</button>
                        <button onClick={() => handleDelete(item.itemId)} className="delete-button">Xóa món</button>
                    </div>
                ))}
            </div>

            {/* Modal để thêm hoặc chỉnh sửa vật phẩm */}
            {isModalOpen && (
                <div className="modal">
                    <div className="modal-content">
                        <h3>{editItem ? 'Edit Item' : 'Thêm món'}</h3>
                        <form onSubmit={handleSubmit}>
                            <label>
                                Tên món:
                                <input
                                    type="text"
                                    name="name"
                                    value={formData.name}
                                    onChange={handleChange}
                                    required
                                />
                            </label>
                            <label>
                                Giá:
                                <input
                                    type="number"
                                    name="price"
                                    value={formData.price}
                                    onChange={handleChange}
                                    required
                                />
                            </label>
                            <button type="submit" className="save-button">{editItem ? 'Update' : 'Thêm'}</button>
                            <button type="button" onClick={closeModal} className="cancel-button">Hủy</button>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
}

export default Item;
