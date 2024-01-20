import React, { useEffect, useState } from 'react';
import './Item.css';

const Item = () => {
    const [item, setItem] = useState([]);
    useEffect(() => {
        fetch('http://localhost:5236/api/Item/GetAllItems') 
            .then(response => response.json())
            .then(data => setItem(data));
    }, []);
    
    return (
        <div>
            <h1 className='i-a'>Items</h1>
            <div className='quests'>
                {item.map(item => (
                    <button key={item.id} 
                    style={{
                        backgroundColor: '#ADD8E6',
                        backgroundSize: 'cover',
                        width: '200px',
                        height: '200px', 
                        border: 'none',
                        cursor: 'pointer',
                        margin: '3rem',
                        color: 'white'
                    }}
                    >
                        Id: [{item.item.id}]<br/>
                        {item.item.properties.name}<br/>
                        ({item.item.labels[1]})<br/>
                        effect: {item.item.properties.effect}<br/>
                        weight: {item.item.properties.weight}<br/>
                        type: {item.item.properties.type}<br/>
                        value: {item.item.properties.value}<br/>
                        dimensions: {item.item.properties.dimensions}<br/>
                        level: {item.item.properties.level}<br/>
                        slot: {item.item.properties.slot}<br/>
                        quality: {item.item.properties.quality}<br/>
                    </button>
                ))}
            </div>
        </div>
    );
};

export default Item;