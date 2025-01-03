import axios from 'axios';
// Cấu hình base URL cho toàn bộ ứng dụng
const API_BASE_URL = 'http://localhost:5175/api';

const apiClient = axios.create({
    baseURL: API_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Định nghĩa các hàm gọi API
// API danh sách phim
export const getMovies = () => apiClient.get('/Movie');
export const getMovieById = (id) => apiClient.get(`/Movie/${id}`);
export const createMovie = (data) => apiClient.post('/Movie', data);
export const updateMovie = (id, data) => apiClient.put(`/Movie/${id}`, data);
export const deleteMovie = (id) => apiClient.delete(`/Movie/${id}`);
export const searchMovies = (query) => apiClient.get(`/Movie/Search`, { params: { query } });

// API danh sách giờ chiếu
export const getShowtimesByMovieId = (movieId) => apiClient.get(`/Showtimes/${movieId}`);
export const createShowtime = (data) => apiClient.post('/Showtimes', data);
export const updateShowtime = (showtimeId, data) => apiClient.put(`/Showtimes/${showtimeId}`, data);
export const deleteShowtime = (showtimeId) => apiClient.delete(`/Showtimes/${showtimeId}`);
export const autoHideShowtime = () => apiClient.post('/Showtimes/auto-hide');

// API danh sách rạp chiếu
export const getTheaters = () => apiClient.get('/Theater');
export const getTheaterById = (id) => apiClient.get(`/Theater/${id}`);
export const createTheater = (data) => apiClient.post('/Theater', data);
export const updateTheater = (id, data) => apiClient.put(`/Theater/${id}`, data);
export const deleteTheater = (id) => apiClient.delete(`/Theater/${id}`);

// API danh sách ghế
export const getSeatsByShowtimeId = (showTimeId) => apiClient.get(`/Seats/${showTimeId}`);
export const lockSeat = (data) => apiClient.post('/Seats/lock', data);
export const unlockSeat = (data) => apiClient.post('/Seats/unlock', data);
export const checkSeatAvailability = (data) => apiClient.post('/Seats/CheckAvailability', data);
export const checkSeats = (data) => apiClient.post('/Seats/CheckSeats', data);
export const lockAndContinueSeat = (data) => apiClient.post('/Seats/lock-and-continue', data);
export const getSeatsByOrderId = (orderId) => apiClient.get(`/Seats/order/${orderId}`);
export const autoUnlockSeat = (data) => apiClient.post('/Seats/auto-unlock', data);

// API danh sách vé
export const getTicketsByUserId = (userId) => apiClient.get(`/Ticket/user/${userId}`);
export const createTicket = (data) => apiClient.post('/Ticket', data);
export const getTickets = () => apiClient.get('/Ticket');
export const deleteTicket = (ticketId) => apiClient.delete(`/Ticket/${ticketId}`);

// API danh sách đồ ăn/uống
export const getItems = () => apiClient.get('/Item');
export const getItemById = (id) => apiClient.get(`/Item/${id}`);
export const createItem = (data) => apiClient.post('/Item', data);
export const updateItem = (id, data) => apiClient.put(`/Item/${id}`, data);
export const deleteItem = (id) => apiClient.delete(`/Item/${id}`);

// API đơn hàng
export const createOrderAndTickets = (data) => apiClient.post('/Orders/CreateOrderAndTickets', data);
export const getOrderById = (id) => apiClient.get(`/Orders/${id}`);
export const deleteOrder = (id) => apiClient.delete(`/Orders/${id}`);

// API báo cáo
export const getSalesReport = () => apiClient.get('/Report/SalesReport');
export const exportSalesReportToExcel = () => apiClient.get('/Report/ExportToExcel');

// API thông tin vé
export const getTicketInfoByUserId = (userId) => apiClient.get(`/TicketInfo/user/${userId}`);
export const getTicketInfoById = (id) => apiClient.get(`/TicketInfo/${id}`);
export const deleteTicketInfo = (id) => apiClient.delete(`/TicketInfo/${id}`);
export const createTicketInfo = (data) => apiClient.post('/TicketInfo', data);

// API người dùng
export const registerUser = (data) => apiClient.post('/Users/register', data);
export const loginUser = (data) => apiClient.post('/Users/login', data);

export default apiClient;

