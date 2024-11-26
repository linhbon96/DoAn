import React, { useState, useEffect } from 'react';
import axios from 'axios';
import './css/ShowtimeManager.css';

function ShowtimeManager() {
  const [movies, setMovies] = useState([]);
  const [theaters, setTheaters] = useState([]);
  const [showtimes, setShowtimes] = useState([]);
  const [selectedMovie, setSelectedMovie] = useState(null);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [newShowtime, setNewShowtime] = useState({
    showDate: '',
    showHour: '',
    theaterId: ''
  });

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
      setMovies(Array.isArray(response.data) ? response.data : []);
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

    // Kiểm tra nếu thiếu các trường cần thiết
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
        fetchShowtimes(selectedMovie.movieId); // Làm mới danh sách giờ chiếu
        setIsModalOpen(false); // Đóng modal sau khi thêm thành công
        setNewShowtime({ showDate: '', showHour: '', theaterId: '' }); // Đặt lại các trường của form
      }
    } catch (error) {
      console.error('Error adding showtime:', error);
    }
  };

  const handleDeleteShowtime = async (showtimeId) => {
    if (!window.confirm('Bạn có chắc muốn xóa giờ chiếu này không?')) return;
    try {
      await axios.delete(`http://localhost:5175/api/ShowTimes/${showtimeId}`);
      fetchShowtimes(selectedMovie.movieId); // Làm mới danh sách giờ chiếu sau khi xóa
    } catch (error) {
      console.error('Error deleting showtime:', error);
    }
  };

  return (
    <div>
      <h1>Quản Lý Giờ Chiếu</h1>

      <div className="movie-selection">
        <label>Chọn Phim:</label>
        <select
          value={selectedMovie ? selectedMovie.movieId : ''}
          onChange={(e) => {
            const selected = movies.find(movie => movie.movieId === parseInt(e.target.value));
            setSelectedMovie(selected);
            setShowtimes([]);
          }}
        >
          <option value="">Chọn Phim</option>
          {movies.map(movie => (
            <option key={movie.movieId} value={movie.movieId}>
              {movie.title}
            </option>
          ))}
        </select>

        {selectedMovie && (
          <button onClick={() => setIsModalOpen(true)} className="add-showtime-button">
            Thêm Giờ Chiếu
          </button>
        )}
      </div>

      {selectedMovie && (
        <div>
          <h2>Danh Sách Giờ Chiếu cho {selectedMovie.title}</h2>
          <table className="showtime-table">
            <thead>
              <tr>
                <th>Ngày Chiếu</th>
                <th>Giờ Chiếu</th>
                <th>Phòng Chiếu</th>
                <th>Hành Động</th>
              </tr>
            </thead>
            <tbody>
              {showtimes.length > 0 ? (
                showtimes.map(showtime => (
                  <tr key={showtime.showtimeId}>
                    <td>{new Date(showtime.showDate).toLocaleDateString()}</td>
                    <td>{showtime.showHour}</td>
                    <td>{showtime.theaterName}</td>
                    <td>
                      <button onClick={() => handleDeleteShowtime(showtime.showtimeId)} className="delete-button">
                        Xóa
                      </button>
                    </td>
                  </tr>
                ))
              ) : (
                <tr>
                  <td colSpan="4">Chưa có giờ chiếu cho phim này</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      )}

      {isModalOpen && (
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
              <button type="submit">Xác Nhận</button>
              <button type="button" onClick={() => setIsModalOpen(false)}>
                Hủy Bỏ
              </button>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}

export default ShowtimeManager;
