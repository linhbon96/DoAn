
// Unit tests for: TicketBooking

import React from 'react'
import { fireEvent, render, screen, waitFor } from '@testing-library/react';
import axios from 'axios';
import TicketBooking from '../TicketBooking';
import "@testing-library/jest-dom";

// Mocking useParams to return a specific movieId
jest.mock("react-router-dom", () => ({
  ...jest.requireActual("react-router-dom"),
  useParams: () => ({ movieId: '123' }),
}));

// Mocking axios
jest.mock("axios");

describe('TicketBooking() TicketBooking method', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  describe('Happy Path', () => {
    it('should fetch and display showtimes for a given movieId', async () => {
      // Mocking axios response for showtimes
      axios.get.mockResolvedValueOnce({
        data: [
          { showtimeId: '1', showDate: '2023-10-01', showHour: '18:00' },
          { showtimeId: '2', showDate: '2023-10-02', showHour: '20:00' },
        ],
      });

      render(<TicketBooking />);

      // Wait for showtimes to be displayed
      await waitFor(() => {
        expect(screen.getByText('01/10/2023 18:00')).toBeInTheDocument();
        expect(screen.getByText('02/10/2023 20:00')).toBeInTheDocument();
      });
    });

    it('should allow seat selection and display total price', async () => {
      // Mocking axios responses for theater info and seats
      axios.get.mockResolvedValueOnce({
        data: { rows: 5, columns: 5 },
      }).mockResolvedValueOnce({
        data: [
          { id: 'A1', isAvailable: true },
          { id: 'A2', isAvailable: true },
        ],
      });

      render(<TicketBooking />);

      // Simulate selecting a showtime
      fireEvent.click(screen.getByText('01/10/2023 18:00'));

      // Wait for seats to be displayed
      await waitFor(() => {
        expect(screen.getByText('A1')).toBeInTheDocument();
        expect(screen.getByText('A2')).toBeInTheDocument();
      });

      // Simulate seat selection
      fireEvent.click(screen.getByText('A1'));

      // Check total price
      expect(screen.getByText('Tổng Tiền: 100000 VND')).toBeInTheDocument();
    });

    it('should successfully book selected seats', async () => {
      // Mocking axios responses for booking and order
      axios.post.mockResolvedValueOnce({
        status: 200,
        data: { orderId: 'order123' },
      });
      axios.get.mockResolvedValueOnce({
        data: { orderId: 'order123', total: 100000 },
      });

      render(<TicketBooking />);

      // Simulate selecting a showtime and a seat
      fireEvent.click(screen.getByText('01/10/2023 18:00'));
      await waitFor(() => fireEvent.click(screen.getByText('A1')));

      // Simulate booking
      fireEvent.click(screen.getByText('Đặt Vé'));

      // Wait for order summary to be displayed
      await waitFor(() => {
        expect(screen.getByText('Order Summary')).toBeInTheDocument();
      });
    });
  });

  describe('Edge Cases', () => {
    it('should handle errors when fetching showtimes', async () => {
      // Mocking axios to throw an error
      axios.get.mockRejectedValueOnce(new Error('Network Error'));

      render(<TicketBooking />);

      // Check for error message in console (mocked)
      await waitFor(() => {
        expect(console.error).toHaveBeenCalledWith('Error fetching showtimes:', expect.any(Error));
      });
    });

    it('should handle errors when booking seats', async () => {
      // Mocking axios responses for seat selection and booking error
      axios.get.mockResolvedValueOnce({
        data: [
          { id: 'A1', isAvailable: true },
        ],
      });
      axios.post.mockRejectedValueOnce(new Error('Booking Error'));

      render(<TicketBooking />);

      // Simulate selecting a showtime and a seat
      fireEvent.click(screen.getByText('01/10/2023 18:00'));
      await waitFor(() => fireEvent.click(screen.getByText('A1')));

      // Simulate booking
      fireEvent.click(screen.getByText('Đặt Vé'));

      // Check for error message in console (mocked)
      await waitFor(() => {
        expect(console.error).toHaveBeenCalledWith('Error booking seats:', expect.any(Error));
      });
    });
  });
});

// End of unit tests for: TicketBooking
