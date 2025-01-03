import React, { useEffect, useState } from 'react';
import './css/Item.css';
import { getItems, createItem, updateItem, deleteItem } from '../services/apiService';

function Item() {
    const [items, setItems] = useState([]);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [editItem, setEditItem] = useState(null); // Đối tượng đang chỉnh sửa hoặc thêm mới
    const [formData, setFormData] = useState({ name: '', price: '' });
    const [error, setError] = useState(null);
    const [searchTerm, setSearchTerm] = useState(''); // Tìm kiếm theo tên món
    const [currentPage, setCurrentPage] = useState(1); // Trang hiện tại
    const itemsPerPage = 4; // Số lượng món trên mỗi trang

    useEffect(() => {
        fetchItems();
    }, []);

    // Lấy danh sách vật phẩm từ API
    const fetchItems = async () => {
        try {
            const response = await getItems();
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
                await updateItem(editItem.itemId, formData);
            } else {
                // Thêm vật phẩm mới
                await createItem(formData);
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
                await deleteItem(itemId);
                fetchItems();
                closeModal();
            } catch (error) {
                console.error('Error deleting item:', error);
                setError('Failed to delete item');
            }
        }
    };

    // Xử lý tìm kiếm món
    const filteredItems = items.filter((item) =>
        item.name.toLowerCase().includes(searchTerm.toLowerCase())
    );

    // Tính toán số trang
    const totalPages = Math.ceil(filteredItems.length / itemsPerPage);

    // Lấy món của trang hiện tại
    const currentItems = filteredItems.slice(
        (currentPage - 1) * itemsPerPage,
        currentPage * itemsPerPage
    );

    return (
        <div className="item-manager">
            <h2>Quản lý món</h2>
            {error && <p className="error-message">{error}</p>}

            {/* Thanh tìm kiếm */}
            <div className="search-container">
                <input
                    type="text"
                    placeholder="Tìm kiếm món..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="search-input"
                />
            </div>

            {/* Nút Thêm Mới Món */}
            <button onClick={() => openModal()} className="add-item-button">Thêm món</button>

            {/* Thẻ danh sách món */}
            <div className="item-list">
                {currentItems.length > 0 ? (
                    currentItems.map((item) => (
                        <div key={item.itemId} className="item-card" onClick={() => openModal(item)}>
                            <p><strong>Tên món:</strong> {item.name}</p>
                            <p><strong>Giá:</strong> {item.price} VND</p>
                        </div>
                    ))
                ) : (
                    <p>Không có món nào</p>
                )}
            </div>

            {/* Nút phân trang */}
            <div className="pagination">
                <button
                    onClick={() => setCurrentPage(currentPage - 1)}
                    disabled={currentPage === 1}
                >
                    Trước
                </button>
                {Array.from({ length: totalPages }, (_, index) => (
                    <button
                        key={index + 1}
                        onClick={() => setCurrentPage(index + 1)}
                        className={currentPage === index + 1 ? 'active' : ''}
                    >
                        {index + 1}
                    </button>
                ))}
                <button
                    onClick={() => setCurrentPage(currentPage + 1)}
                    disabled={currentPage === totalPages}
                >
                    Sau
                </button>
            </div>

            {/* Modal để thêm hoặc chỉnh sửa vật phẩm */}
            {isModalOpen && (
                <div className="modal">
                    <div className="modal-content">
                        <h3>{editItem ? 'Chỉnh sửa món' : 'Thêm món'}</h3>
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
                            <div className="modal-buttons">
                                <button type="submit" className="save-button">{editItem ? 'Cập nhật' : 'Thêm'}</button>
                                <button type="button" onClick={closeModal} className="cancel-button">Hủy</button>
                                {editItem && (
                                    <button type="button" onClick={() => handleDelete(editItem.itemId)} className="delete-button">Xóa</button>
                                )}
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
}

export default Item;

