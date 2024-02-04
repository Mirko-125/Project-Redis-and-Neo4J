import React, { useEffect, useState } from 'react';
import './Marketplace.css';
import '../../styling/CrudContainer.css';

function Marketplace() {
    const [marketplace, setMarketplaces] = useState([]);
    const [zone, setZone] = useState('');
    const [itemCount, setItemCount] = useState(0);
    const [restockCycle, setRestockCycle] = useState(0);
    const [itemName, setItemName] = useState('');
    const [marketZone, setMarketZone] = useState(''); 
    const [marketplaceID, setMarketplaceID] = useState(0);
    const [items, setItems] = useState([]);
    
    useEffect(() => {
        fetch(`http://localhost:5236/api/Marketplace/GetAll`)
            .then(response => response.json())
            .then(data => setMarketplaces(data));
    }, []);

    const handleMarketClick = (market) => {
        fetch(`http://localhost:5236/api/Marketplace/GetOne?zone=${market.zone}`)
            .then(response => response.json())
            .then(data => {setItems(data.items); console.log(data)})
            .then(console.log(items));
    }
    
    const handleCreateMarketplace = () => {
        const marketplaceData = {
            zone: zone,
            itemCount: itemCount,
            restockCycle: restockCycle
        };

        fetch(`http://localhost:5236/api/Marketplace`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(marketplaceData)
        })
            .then(response => response.json())
            .then(data => {
                // Handle the response data if needed
                console.log(data);
            })
            .catch(error => {
                // Handle the error if needed
                console.error(error);
            });
    };

    const handleUpdateMarketplace = () => {
        const marketplaceData = {
            markketplaceID: marketplaceID,
            zone: zone,
            restockCycle: restockCycle
        };

        fetch(`http://localhost:5236/api/Marketplace/Update`, {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(marketplaceData)
        })
            .then(response => response.json())
            .then(data => {
                // Handle the response data if needed
                console.log(data);
            })
            .catch(error => {
                // Handle the error if needed
                console.error(error);
            });
    };

    const handleDeleteMarketplace = () => {
        fetch(`http://localhost:5236/api/Marketplace?zone=${zone}`, {
            method: 'DELETE'
        })
            .then(response => response.json())
            .then(data => {
                // Handle the response data if needed
                console.log(data);
            })
            .catch(error => {
                // Handle the error if needed
                console.error(error);
            });
    };

    const handleAddItemToMarketplace = () => {
        fetch(`http://localhost:5236/api/Marketplace/AddItem?zoneName=${marketZone}&itemName=${itemName}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        })
            .then(response => response.json())
            .then(data => {
                // Handle the response data if needed
                console.log(data);
            })
            .catch(error => {
                // Handle the error if needed
                console.error(error);
            });
    };

    return (
        <div className="wrap-m">
            <h1 className='i-mp'>Markets</h1>
                <div className="markets">
                    {marketplace.map(marketplace => (
                        <button key={marketplace.name} onClick={() => handleMarketClick(marketplace)} 
                        style={{
                            backgroundSize: 'cover',
                            width: '200px', 
                            height: '100px', 
                            border: 'none',
                            cursor: 'pointer',
                            margin: '3rem'
                        }}
                        >
                            {marketplace.zone}
                        </button>
                    ))}
                </div>
            <div className='crud-container'>
                <div>
                    <h1 className='i-a'>Admin: Add a new Marketplace</h1>
                    <div className='input-container'>
                        <input

                            type="text"
                            placeholder="Enter zone"
                            onChange={(e) => setZone(e.target.value)}
                        />
                        <input

                            type="number"
                            placeholder="Enter item count"
                            onChange={(e) => setItemCount(parseInt(e.target.value))}
                        />
                        <input

                            type="number"
                            placeholder="Enter restock cycle"
                            onChange={(e) => setRestockCycle(parseInt(e.target.value))}
                        />
                        <button className='blue-bg' onClick={handleCreateMarketplace}>Create Marketplace</button>
                    </div>
                </div>
                <div>
                    <h1 className='i-a'>Admin: Move a market</h1>
                    <div className='input-container'>
                        <input
                            type="text"
                            placeholder="Enter zone"
                            onChange={(e) => setZone(e.target.value)}
                        />
                        <input
                            type="number"
                            placeholder="Enter market's original ID"
                            onChange={(e) => setMarketplaceID(parseInt(e.target.value))}
                        />
                        <input
                            type="number"
                            placeholder="Enter restock cycle"
                            onChange={(e) => setRestockCycle(parseInt(e.target.value))}
                        />
                        <button className='green-bg' onClick={handleUpdateMarketplace}>Update marketplace</button>
                    </div>
                </div>
                <div>
                    <h1 className='i-a'>Add an item to the selected marketplace</h1>
                    <div className='input-container'>
                        <input
                            type="text"
                            placeholder="Enter item name"
                            onChange={(e) => setItemName(e.target.value)}
                        />
                        <input
                            type="text"
                            placeholder="Enter market's zone"
                            onChange={(e) => setMarketZone(e.target.value)}
                        />
                        <button className='violet-bg' onClick={handleAddItemToMarketplace}>Add item</button>
                    </div>
                </div>
                <div>
                    <h1 className='i-a'>Delete marketplaces in a certain zone</h1>
                    <div className='input-container'>
                        <input
                            type="text"
                            placeholder="Enter zone"
                            onChange={(e) => setMarketZone(e.target.value)}
                        />
                        <button className='red-bg' onClick={handleDeleteMarketplace}>Delete marketplace</button>
                    </div>
                </div>
            </div>        
            <h1 className='i-a'>Items</h1>
            <div className='items'>
                {items.map((item, idx) => (
                    <button key={idx} 
                    style={{
                        backgroundColor: item.$type === 'Gear' ? '#043399' : '#C95000',
                        backgroundSize: 'cover',
                        width: '200px',
                        height: '200px', 
                        border: 'none',
                        cursor: 'pointer',
                        margin: '3rem',
                        color: 'white'
                    }}
                    >
                        {item.name}<br/>
                        weight: {item.weight}<br/>
                        type: {item.type}<br/>
                        value: {item.value}<br/>
                        dimensions: {item.dimensions}<br/>
                        {item.$type === 'Gear' && (
                        <>
                            level: {item.attributes.level}<br/>
                            slot: {item.slot}<br/>
                            quality: {item.quality}<br/>
                        </>
                        )}
                        {item.$type !== 'Gear' && (
                        <>
                            effect: {item.effect}<br />
                        </>
                        )}
                    </button>
            ))}
            </div>      
        </div>
    );
}

export default Marketplace;
