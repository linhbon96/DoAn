import React, { useState, useEffect } from 'react';
import { getMovies, createMovie, updateMovie, deleteMovie } from '../services/apiService';
import './css/AdminDashboard.css';

function AdminDashboard() {
    const [movies, setMovies] = useState([]);
    const [selectedMovie, setSelectedMovie] = useState(null);
    const [updatedMovie, setUpdatedMovie] = useState({
        title: '',
        description: '',
        genre: '',
        duration: '',
        releaseDate: '',
        imageUrl: ''
    });
    const [isModalOpen, setIsModalOpen] = useState(false); // Trạng thái mở/đóng modal chỉnh sửa
    const [isAddModalOpen, setIsAddModalOpen] = useState(false); // Trạng thái modal thêm mới phim
    const [newMovie, setNewMovie] = useState({
        title: '',
        description: '',
        genre: '',
        duration: '',
        releaseDate: '',
        imageUrl: ''
    });
    const [searchTerm, setSearchTerm] = useState(''); // Tìm kiếm theo tên phim
    const [currentPage, setCurrentPage] = useState(1); // Trang hiện tại
    const moviesPerPage = 4; // Số lượng phim trên mỗi trang

    useEffect(() => {
        fetchMovies();
    }, []);

    // Fetch danh sách phim từ API
    const fetchMovies = async () => {
        try {
            const response = await getMovies();
            setMovies(response.data);
        } catch (error) {
            console.error('Error fetching movies:', error);
        }
    };

    // Mở modal chỉnh sửa
    const openModal = (movie) => {
        setSelectedMovie(movie);
        setUpdatedMovie({
            title: movie.title,
            description: movie.description,
            genre: movie.genre,
            duration: movie.duration,
            releaseDate: movie.releaseDate,
            imageUrl: movie.imageUrl
        });
        setIsModalOpen(true);
    };

    // Đóng modal
    const closeModal = () => {
        setIsModalOpen(false);
        setIsAddModalOpen(false);
        setSelectedMovie(null);
    };

    // Cập nhật thông tin phim
    const handleUpdateMovie = async (e) => {
        e.preventDefault();
        try {
            const response = await updateMovie(selectedMovie.movieId, updatedMovie);

            if (response.status === 200) {
                fetchMovies(); // Cập nhật lại danh sách phim
                closeModal();  // Đóng modal
            }
        } catch (error) {
            console.error('Error updating movie:', error);
        }
    };

    // Thêm mới phim
    const handleAddMovie = async (e) => {
        e.preventDefault();
        try {
            const response = await createMovie(newMovie);
            if (response.status === 201) {
                fetchMovies(); // Cập nhật lại danh sách phim
                closeModal();  // Đóng modal thêm mới
            }
        } catch (error) {
            console.error('Error adding movie:', error);
        }
    };

    // Xóa phim
    const handleDeleteMovie = async (movieId) => {
        const confirmDelete = window.confirm("Bạn có chắc muốn xóa phim này?");
        if (confirmDelete) {
            try {
                const response = await deleteMovie(movieId);
                if (response.status === 200) {
                    fetchMovies(); // Cập nhật lại danh sách phim sau khi xóa
                }
            } catch (error) {
                console.error('Error deleting movie:', error);
            }
        }
    };

    // Xử lý tìm kiếm phim
    const filteredMovies = movies.filter((movie) =>
        movie.title.toLowerCase().includes(searchTerm.toLowerCase()) ||
        movie.genre.toLowerCase().includes(searchTerm.toLowerCase())
    );

    // Tính toán số trang
    const totalPages = Math.ceil(filteredMovies.length / moviesPerPage);

    // Lấy phim của trang hiện tại
    const currentMovies = filteredMovies.slice(
        (currentPage - 1) * moviesPerPage,
        currentPage * moviesPerPage
    );

    return (
        <div>
            <h1>Admin Dashboard</h1>

            {/* Thanh tìm kiếm */}
            <div className="search-container">
                <input
                    type="text"
                    placeholder="Tìm kiếm phim..."
                    value={searchTerm}
                    onChange={(e) => setSearchTerm(e.target.value)}
                    className="search-input"
                />
            </div>

            {/* Nút Thêm Mới Phim */}
            <button onClick={() => setIsAddModalOpen(true)} className="add-movie-button">
                Thêm Mới
            </button>

            {/* Thẻ danh sách phim */}
            <div className="movie-cards-container">
                {currentMovies.length > 0 ? (
                    currentMovies.map((movie) => (
                        <div key={movie.movieId} className="movie-card" onClick={() => openModal(movie)}>
                            <img src={movie.imageUrl} alt={`${movie.title} cover`} className="movie-cover" />
                            <h3>{movie.title}</h3>
                            <p>{movie.genre}</p>
                            <p>{movie.duration} phút</p>
                            <p>{new Date(movie.releaseDate).toLocaleDateString()}</p>
                        </div>
                    ))
                ) : (
                    <p>Không có phim nào</p>
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

            {/* Modal để chỉnh sửa thông tin phim */}
            {isModalOpen && selectedMovie && (
                <div className="modal">
                    <div className="modal-content">
                        <h2>Chỉnh Sửa Phim</h2>
                        <form onSubmit={handleUpdateMovie}>
                            <div>
                                <label>Tên Phim:</label>
                                <input
                                    type="text"
                                    value={updatedMovie.title}
                                    onChange={(e) => setUpdatedMovie({ ...updatedMovie, title: e.target.value })}
                                />
                            </div>
                            <div>
                                <label>Mô Tả:</label>
                                <textarea
                                    value={updatedMovie.description}
                                    onChange={(e) => setUpdatedMovie({ ...updatedMovie, description: e.target.value })}
                                />
                            </div>
                            <div>
                                <label>Thể Loại:</label>
                                <input
                                    type="text"
                                    value={updatedMovie.genre}
                                    onChange={(e) => setUpdatedMovie({ ...updatedMovie, genre: e.target.value })}
                                />
                            </div>
                            <div>
                                <label>Thời Lượng:</label>
                                <input
                                    type="number"
                                    value={updatedMovie.duration}
                                    onChange={(e) => setUpdatedMovie({ ...updatedMovie, duration: e.target.value })}
                                />
                            </div>
                            <div>
                                <label>Ngày Phát Hành:</label>
                                <input
                                    type="datetime-local"
                                    value={updatedMovie.releaseDate}
                                    onChange={(e) => setUpdatedMovie({ ...updatedMovie, releaseDate: e.target.value })}
                                />
                            </div>
                            <div>
                                <label>Ảnh:</label>
                                <input
                                    type="text"
                                    value={updatedMovie.imageUrl}
                                    onChange={(e) => setUpdatedMovie({ ...updatedMovie, imageUrl: e.target.value })}
                                />
                            </div>
                            <div className="modal-buttons">
                                <button type="submit">Cập Nhật Phim</button>
                                <button type="button" onClick={closeModal}>Đóng</button>
                            </div>
                        </form>
                    </div>
                </div>
            )}

            {/* Modal để thêm mới phim */}
            {isAddModalOpen && (
                <div className="modal">
                    <div className="modal-content">
                        <h2>Thêm Mới Phim</h2>
                        <form onSubmit={handleAddMovie}>
                            <div>
                                <label>Tên Phim:</label>
                                <input
                                    type="text"
                                    value={newMovie.title}
                                    onChange={(e) => setNewMovie({ ...newMovie, title: e.target.value })}
                                />
                            </div>
                            <div>
                                <label>Mô Tả:</label>
                                <textarea
                                    value={newMovie.description}
                                    onChange={(e) => setNewMovie({ ...newMovie, description: e.target.value })}
                                />
                            </div>
                            <div>
                                <label>Thể Loại:</label>
                                <input
                                    type="text"
                                    value={newMovie.genre}
                                    onChange={(e) => setNewMovie({ ...newMovie, genre: e.target.value })}
                                />
                            </div>
                            <div>
                                <label>Thời Lượng:</label>
                                <input
                                    type="number"
                                    value={newMovie.duration}
                                    onChange={(e) => setNewMovie({ ...newMovie, duration: e.target.value })}
                                />
                            </div>
                            <div>
                                <label>Ngày Phát Hành:</label>
                                <input
                                    type="datetime-local"
                                    value={newMovie.releaseDate}
                                    onChange={(e) => setNewMovie({ ...newMovie, releaseDate: e.target.value })}
                                />
                            </div>
                            <div>
                                <label>Ảnh:</label>
                                <input
                                    type="text"
                                    value={newMovie.imageUrl}
                                    onChange={(e) => setNewMovie({ ...newMovie, imageUrl: e.target.value })}
                                />
                            </div>
                            <div className="modal-buttons">
                                <button type="submit">Thêm Phim</button>
                                <button type="button" onClick={closeModal}>Đóng</button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
}

export default AdminDashboard;

