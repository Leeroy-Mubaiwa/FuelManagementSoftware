// SPDX-License-Identifier: MIT
pragma solidity ^0.8.19;

/// @title FuelTransactionLedger
/// @notice Immutable on-chain ledger for PetroChain fuel transactions
contract FuelTransactionLedger {

    struct FuelRecord {
        string transactionNumber;
        uint256 stationId;
        uint256 pumpId;
        uint256 quantity;     // quantity * 100 (2 decimal places)
        uint256 amount;       // amount * 100 (2 decimal places)
        string cardNumberHash;
        uint256 timestamp;
        address recorder;
    }

    address public owner;
    FuelRecord[] public records;

    // Store index+1 so 0 means "not found"
    mapping(bytes32 => uint256) private _lookup;

    event TransactionRecorded(
        uint256 indexed recordIndex,
        string transactionNumber,
        uint256 stationId,
        uint256 pumpId,
        uint256 quantity,
        uint256 amount,
        uint256 timestamp
    );

    constructor() {
        owner = msg.sender;
    }

    function recordTransaction(
        string memory _txNumber,
        uint256 _stationId,
        uint256 _pumpId,
        uint256 _quantity,
        uint256 _amount,
        string memory _cardHash
    ) public returns (uint256) {
        bytes32 key = keccak256(bytes(_txNumber));
        require(_lookup[key] == 0, "Transaction already recorded");

        uint256 index = records.length;
        records.push(FuelRecord({
            transactionNumber: _txNumber,
            stationId: _stationId,
            pumpId: _pumpId,
            quantity: _quantity,
            amount: _amount,
            cardNumberHash: _cardHash,
            timestamp: block.timestamp,
            recorder: msg.sender
        }));

        _lookup[key] = index + 1;

        emit TransactionRecorded(
            index, _txNumber, _stationId, _pumpId,
            _quantity, _amount, block.timestamp
        );

        return index;
    }

    function getRecordCount() public view returns (uint256) {
        return records.length;
    }

    function getRecord(uint256 _index) public view returns (
        string memory transactionNumber,
        uint256 stationId,
        uint256 pumpId,
        uint256 quantity,
        uint256 amount,
        string memory cardNumberHash,
        uint256 timestamp,
        address recorder
    ) {
        require(_index < records.length, "Index out of bounds");
        FuelRecord storage r = records[_index];
        return (
            r.transactionNumber,
            r.stationId,
            r.pumpId,
            r.quantity,
            r.amount,
            r.cardNumberHash,
            r.timestamp,
            r.recorder
        );
    }

    function verifyTransaction(string memory _txNumber) public view returns (
        bool exists,
        uint256 index
    ) {
        uint256 val = _lookup[keccak256(bytes(_txNumber))];
        if (val > 0) {
            return (true, val - 1);
        }
        return (false, 0);
    }
}

