import React from 'react';
import { BrowserRouter as Router, Routes, Route } from 'react-router-dom';
import { AuthProvider } from './components/AuthContext'; 
import Navbar from './components/Navbar';
import MovieList from './components/MovieList';
import MovieDetail from './components/MovieDetail';
import BookingPage from './components/BookingPage';
import TicketBooking from './components/TicketBooking';
import Login from './components/Login';
import Register from './components/Register';
import AdminDashboard from './components/AdminDashboard';
import ShowtimeManager from './components/ShowtimeManager';
import TicketInfo from './components/TicketInfo';
import Item from './components/Item';
import './App.css';
import OrderSummary from './components/OrderSummary';

function App() {
    return (
        <Router>
            <AuthProvider> {/* Cung cấp AuthContext cho toàn bộ ứng dụng */}
                <Navbar /> {/* Navbar sẽ có quyền truy cập vào AuthContext */}
                <Routes>
                    <Route path="/" element={<MovieList />} />
                    <Route path="/movie/:movieId" element={<MovieDetail />} />
                    <Route path="/book/:showtimeId" element={<BookingPage />} />
                    <Route path="/ticketbooking" element={<TicketBooking />} />
                    <Route path="/ticketinfo/:userId" element={<TicketInfo />} />
                    <Route path="/item" element={<Item />} />
                    <Route path="/login" element={<Login />} />
                    <Route path="/register" element={<Register />} />
                    <Route path="/admin" element={<AdminDashboard />} />
                    <Route path="/showtime" element={<ShowtimeManager />} />
                    <Route path="/order" element={<OrderSummary />} />
                </Routes>
            </AuthProvider>
        </Router>
    );
}

export default App;
