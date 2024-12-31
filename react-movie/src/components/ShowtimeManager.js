import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './css/ShowtimeManager.css';

function ShowtimeManager() {
    const [movies, setMovies] = useState([]);
    const [theaters, setTheaters] = useState([]);
    const [showtimes, setShowtimes] = useState([]);
    const [selectedMovie, setSelectedMovie] = useState(null);
    const [isModalOpen, setIsModalOpen] = useState(false);
    const [isAddShowtimeModalOpen, setIsAddShowtimeModalOpen] = useState(false);
    const [newShowtime, setNewShowtime] = useState({
        showDate: '',
        showHour: '',
        theaterId: ''
    });
    const [searchTerm, setSearchTerm] = useState(''); // Thêm state cho từ khóa tìm kiếm
    const [currentPage, setCurrentPage] = useState(1);
    const moviesPerPage = 6;

    useEffect(() => {
        fetchMovies();
        fetchTheaters();
    }, []);

    useEffect(() => {
        if (selectedMovie) {
            fetchShowtimes(selectedMovie.movieId);
        }
    }, [selectedMovie]);

    const fetchMovies = async () => {
        try {
            const response = await axios.get('http://localhost:5175/api/Movie');
            const sortedMovies = Array.isArray(response.data) ? response.data.sort((a, b) => new Date(b.releaseDate) - new Date(a.releaseDate)) : [];
            setMovies(sortedMovies);
        } catch (error) {
            console.error('Error fetching movies:', error);
        }
    };

    const fetchTheaters = async () => {
        try {
            const response = await axios.get('http://localhost:5175/api/Theater');
            setTheaters(Array.isArray(response.data) ? response.data : []);
        } catch (error) {
            console.error('Error fetching theaters:', error);
        }
    };

    const fetchShowtimes = async (movieId) => {
        try {
            const response = await axios.get(`http://localhost:5175/api/ShowTimes/${movieId}`);
            setShowtimes(Array.isArray(response.data) ? response.data : []);
        } catch (error) {
            console.error('Error fetching showtimes:', error);
        }
    };

    const handleAddShowtime = async (e) => {
        e.preventDefault();

        if (!newShowtime.theaterId || !newShowtime.showDate || !newShowtime.showHour) {
            alert('Vui lòng nhập đầy đủ thông tin phòng chiếu, ngày chiếu và giờ chiếu.');
            return;
        }

        const showtimeData = {
            movieId: selectedMovie.movieId,
            theaterId: newShowtime.theaterId,
            showDate: new Date(newShowtime.showDate).toISOString(),
            showHour: `${newShowtime.showHour}:00`
        };

        try {
            const response = await axios.post('http://localhost:5175/api/ShowTimes', showtimeData);
            if (response.status === 201) {
                fetchShowtimes(selectedMovie.movieId);
                setIsAddShowtimeModalOpen(false);
                setNewShowtime({ showDate: '', showHour: '', theaterId: '' });
            }
        } catch (error) {
            console.error('Error adding showtime:', error);
        }
    };

    const handleDeleteShowtime = async (showtimeId) => {
        if (!window.confirm('Bạn có chắc muốn xóa giờ chiếu này không?')) return;
        try {
            await axios.delete(`http://localhost:5175/api/ShowTimes/${showtimeId}`);
            fetchShowtimes(selectedMovie.movieId);
        } catch (error) {
            console.error('Error deleting showtime:', error);
        }
    };

    const indexOfLastMovie = currentPage * moviesPerPage;
    const indexOfFirstMovie = indexOfLastMovie - moviesPerPage;
    const filteredMovies = movies.filter(movie =>
        movie.title.toLowerCase().includes(searchTerm.toLowerCase())
    );
    const currentMovies = filteredMovies.slice(indexOfFirstMovie, indexOfLastMovie);

    const paginate = (pageNumber) => setCurrentPage(pageNumber);

    return (
        <div>
            <h1>Quản Lý Giờ Chiếu</h1>

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

            <div className="movie-cards-container">
                {currentMovies.map(movie => (
                    <div key={movie.movieId} className="movie-card" onClick={() => {
                        setSelectedMovie(movie);
                        setIsModalOpen(true);
                    }}>
                        <img src={movie.imageUrl} alt={movie.title} />
                        <h3>{movie.title}</h3>
                        <p>{new Date(movie.releaseDate).toLocaleDateString()}</p>
                    </div>
                ))}
            </div>

            <div className="pagination">
                {Array.from({ length: Math.ceil(filteredMovies.length / moviesPerPage) }, (_, index) => (
                    <button
                        key={index + 1}
                        onClick={() => paginate(index + 1)}
                        disabled={currentPage === index + 1}
                    >
                        {index + 1}
                    </button>
                ))}
            </div>

            {isModalOpen && (
                <div className="modal">
                    <div className="modal-content">
                        <h3>Lịch Chiếu cho {selectedMovie?.title}</h3>
                        <div className="showtime-cards-container">
                            {showtimes.length > 0 ? (
                                showtimes.map(showtime => (
                                    <div key={showtime.showtimeId} className="showtime-card">
                                        <p><strong>Rạp:</strong> {showtime.theaterName}</p>
                                        <p><strong>Ngày Chiếu:</strong> {new Date(showtime.showDate).toLocaleDateString()}</p>
                                        <p><strong>Giờ Chiếu:</strong> {showtime.showHour}</p>
                                        <button onClick={() => handleDeleteShowtime(showtime.showtimeId)} className="delete-button">
                                            Xóa
                                        </button>
                                    </div>
                                ))
                            ) : (
                                <p>Chưa có giờ chiếu cho phim này</p>
                            )}
                        </div>
                        <div className="modal-buttons">
                            <button type="button" onClick={() => setIsAddShowtimeModalOpen(true)}>
                                Thêm Giờ Chiếu
                            </button>
                            <button type="button" onClick={() => setIsModalOpen(false)}>
                                Đóng
                            </button>
                        </div>
                    </div>
                </div>
            )}

            {isAddShowtimeModalOpen && (
                <div className="modal">
                    <div className="modal-content">
                        <h3>Thêm Giờ Chiếu Mới cho {selectedMovie?.title}</h3>
                        <form onSubmit={handleAddShowtime}>
                            <div>
                                <label>Chọn Ngày Chiếu:</label>
                                <input
                                    type="date"
                                    value={newShowtime.showDate}
                                    onChange={(e) => setNewShowtime({ ...newShowtime, showDate: e.target.value })}
                                    required
                                />
                            </div>
                            <div>
                                <label>Chọn Giờ Chiếu:</label>
                                <input
                                    type="time"
                                    value={newShowtime.showHour}
                                    onChange={(e) => setNewShowtime({ ...newShowtime, showHour: e.target.value })}
                                    required
                                />
                            </div>
                            <div>
                                <label>Chọn Phòng Chiếu:</label>
                                <select
                                    value={newShowtime.theaterId}
                                    onChange={(e) => setNewShowtime({ ...newShowtime, theaterId: parseInt(e.target.value) })}
                                    required
                                >
                                    <option value="">Chọn Phòng Chiếu</option>
                                    {theaters.map(theater => (
                                        <option key={theater.theaterId} value={theater.theaterId}>
                                            {theater.name}
                                        </option>
                                    ))}
                                </select>
                            </div>
                            <div className="modal-buttons">
                                <button type="submit">Xác Nhận</button>
                                <button type="button" onClick={() => setIsAddShowtimeModalOpen(false)}>
                                    Hủy Bỏ
                                </button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
}

export default ShowtimeManager;

