import React, { useEffect } from 'react';
import './css/SeatMap.css';

function SeatMap({ rows, columns, seats, toggleSeatSelection, selectedSeats, refreshSeats }) {

    useEffect(() => {
        // Tự động làm mới ghế sau mỗi 30 giây
        const interval = setInterval(() => {
            refreshSeats();
        }, 30000);

        return () => clearInterval(interval); // Cleanup interval khi component unmount
    }, [refreshSeats]);

    const renderSeatMap = () => {
        const seatRows = [];
        for (let row = 0; row < rows; row++) {
            const seatRow = [];
            for (let col = 0; col < columns; col++) {
                const seat = seats.find(
                    (s) => s.row === String.fromCharCode(65 + row) && s.number === col + 1
                );

                const isLocked = seat?.lockedUntil && new Date(seat.lockedUntil) > new Date();
                const isSelected = selectedSeats.some((s) => s.seatId === seat?.seatId);

                seatRow.push(
                    <button
                        key={`${row}-${col}`}
                        className={`seat ${
                            seat
                                ? isLocked
                                    ? 'locked'
                                    : seat.isAvailable
                                        ? isSelected
                                            ? 'selected'
                                            : 'available'
                                        : 'booked'
                                : 'empty'
                        }`}
                        onClick={() => seat && toggleSeatSelection(seat)}
                        disabled={!seat || !seat.isAvailable || isLocked}
                    >
                        {seat ? `${seat.row}${seat.number}` : ''}
                    </button>
                );
            }
            seatRows.push(<div key={row} className="seat-row">{seatRow}</div>);
        }
        return seatRows;
    };

    return (
        <div className="seat-map">
            <h2 className="screen-label">Màn Hình</h2>
            {renderSeatMap()}
        </div>
    );
}

export default SeatMap;
