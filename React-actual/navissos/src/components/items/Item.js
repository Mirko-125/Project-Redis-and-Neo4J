import React, { useEffect, useState } from 'react';
import './Item.css';

const Item = () => {
    const [items, setItem] = useState([]);
    const [gearName, setGearName] = useState('');
    const [gearType, setGearType] = useState('');
    const [gearWeight, setGearWeight] = useState(0);
    const [gearDimensions, setGearDimensions] = useState(0);
    const [gearValue, setGearValue] = useState(0);
    const [gearSlot, setGearSlot] = useState(0);
    const [gearLevel, setGearLevel] = useState(0);
    const [gearQuality, setGearQuality] = useState('');
    const [gearStrength, setGearStrength] = useState(0);
    const [gearAgility, setGearAgility] = useState(0);
    const [gearIntelligence, setGearIntelligence] = useState(0);
    const [gearStamina, setGearStamina] = useState(0);
    const [gearFaith, setGearFaith] = useState(0);
    const [gearExperience, setGearExperience] = useState(0);
    const [consumableName, setConsumableName] = useState('');
    const [consumableType, setConsumableType] = useState('');
    const [consumableWeight, setConsumableWeight] = useState(0);
    const [consumableDimensions, setConsumableDimensions] = useState(0);
    const [consumableValue, setConsumableValue] = useState(0);
    const [consumableEffect, setConsumableEffect] = useState('');
    const [itemName, setItemName] = useState('');

    const createGearData = () => {
        const gearData = {
            name: gearName,
            type: gearType,
            weight: gearWeight,
            dimensions: gearDimensions,
            value: gearValue,
            slot: gearSlot,
            level: gearLevel,
            attributes: {
                level: gearLevel,
                strength: gearStrength,
                agility: gearAgility,
                intelligence: gearIntelligence,
                stamina: gearStamina,
                faith: gearFaith,
                experience: gearExperience
            },
            quality: gearQuality,
        };
        return gearData;
    }

    const updateGearData = () => {
        const gearData = {...createGearData()};
        return gearData;
    }

    const handleCreateGear = () => {
        
        const gearData = createGearData();

        fetch('http://localhost:5236/api/Gear', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(gearData)
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

    const handleDeleteItem = () => {
        fetch(`http://localhost:5236/api/Item?name=${itemName}`, {
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

    const handleCreateConsumable = () => {
        const consumableData = {
            name: consumableName,
            type: consumableType,
            weight: consumableWeight,
            dimensions: consumableDimensions,
            value: consumableValue,
            effect: consumableEffect
        };

        fetch('http://localhost:5236/api/Consumable', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(consumableData)
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

    useEffect(() => {
        fetch('http://localhost:5236/api/Item/GetAll') 
            .then(response => response.json())
            .then(data => setItem(data));
    }, []);
    
    const handleUpdateGear = () => {
        const gearData = updateGearData();

        fetch('http://localhost:5236/api/Gear/Update', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(gearData)
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

    const handleUpdateConsumable = () => {
        const consumableData = {
            name: consumableName,
            type: consumableType,
            weight: consumableWeight,
            dimensions: consumableDimensions,
            value: consumableValue,
            effect: consumableEffect
        };

        fetch('http://localhost:5236/api/Consumable/Update', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(consumableData)
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
        <div>
            <h1 className='i-a'>Items</h1>
            <div className='items'>
                {items.map(item => (
                    <button key={item.name} 
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
            <h1 className='i-m'>Admin: Remove an item</h1>
            <div className='delete-item'>
                <input type="text" placeholder="Enter item name" onChange={e => setItemName(e.target.value)} />
                <button onClick={handleDeleteItem}>Remove item</button>
            </div>
            <div className='split'>
                <div className='gear'>
                    <div className='create-gear'>
                        <h1 className='i-n'>+ Gear</h1>
                        <input className='create-input' type="text" placeholder="Enter gear name" onChange={(e) => setGearName(e.target.value)} />
                        <input className='create-input' type="text" placeholder="Enter gear type" onChange={(e) => setGearType(e.target.value)} />
                        <input className='create-input' type="number" placeholder="Enter gear weight" onChange={(e) => setGearWeight(e.target.value)} />
                        <input className='create-input' type="number" placeholder="Enter gear dimensions" onChange={(e) => setGearDimensions(e.target.value)} />
                        <input className='create-input' type="number" placeholder="Enter gear value" onChange={(e) => setGearValue(e.target.value)} />
                        <input className='create-input' type="number" placeholder="Enter gear slot" onChange={(e) => setGearSlot(e.target.value)} />
                        <input className='create-input' type="number" placeholder="Enter gear level" onChange={(e) => setGearLevel(e.target.value)} />
                        <input className='create-input' type="text" placeholder="Enter gear quality" onChange={(e) => setGearQuality(e.target.value)} />
                        <input className='create-input' type="number" placeholder="Enter gear strength" onChange={(e) => setGearStrength(e.target.value)} />
                        <input className='create-input' type="number" placeholder="Enter gear agility" onChange={(e) => setGearAgility(e.target.value)} />
                        <input className='create-input' type="number" placeholder="Enter gear intelligence" onChange={(e) => setGearIntelligence(e.target.value)} />
                        <input className='create-input' type="number" placeholder="Enter gear stamina" onChange={(e) => setGearStamina(e.target.value)} />
                        <input className='create-input' type="number" placeholder="Enter gear faith" onChange={(e) => setGearFaith(e.target.value)} />
                        <input className='create-input' type="number" placeholder="Enter gear experience" onChange={(e) => setGearExperience(e.target.value)} />
                        <br/>
                        <button className='gear-button' onClick={handleCreateGear}>Create a new Gear</button>
                    </div>
                        
                    <div className='create-gear'>
                        <h1 className='i-n'>Modify Gear</h1>
                        <input className='create-input' type="text" placeholder="Enter gear name" onChange={(e) => setGearName(e.target.value)} />
                        <input className='create-input' type="text" placeholder="Enter gear type" onChange={(e) => setGearType(e.target.value)} />
                        <input className='create-input' type="number" placeholder="Enter gear weight" onChange={(e) => setGearWeight(e.target.value)} />
                        <input className='create-input' type="number" placeholder="Enter gear dimensions" onChange={(e) => setGearDimensions(e.target.value)} />
                        <input className='create-input' type="number" placeholder="Enter gear value" onChange={(e) => setGearValue(e.target.value)} />
                        <input className='create-input' type="number" placeholder="Enter gear slot" onChange={(e) => setGearSlot(e.target.value)} />
                        <input className='create-input' type="number" placeholder="Enter gear level" onChange={(e) => setGearLevel(e.target.value)} />
                        <input className='create-input' type="text" placeholder="Enter gear quality" onChange={(e) => setGearQuality(e.target.value)} />
                        <input className='create-input' type="number" placeholder="Enter gear strength" onChange={(e) => setGearStrength(e.target.value)} />
                        <input className='create-input' type="number" placeholder="Enter gear agility" onChange={(e) => setGearAgility(e.target.value)} />
                        <input className='create-input' type="number" placeholder="Enter gear intelligence" onChange={(e) => setGearIntelligence(e.target.value)} />
                        <input className='create-input' type="number" placeholder="Enter gear stamina" onChange={(e) => setGearStamina(e.target.value)} />
                        <input className='create-input' type="number" placeholder="Enter gear faith" onChange={(e) => setGearFaith(e.target.value)} />
                        <input className='create-input' type="number" placeholder="Enter gear experience" onChange={(e) => setGearExperience(e.target.value)} />
                        <br/>
                        <button className='gear-button' onClick={handleUpdateGear}>Modify gear</button>
                    </div>    
                </div>
                <div className='consumable-div'>
                
                <div className='create-consumable'>
                    <h1 className='i-n'>+ Cons.</h1>
                    <input className='create-input' type="text" placeholder="Enter consumable name" onChange={(e) => setConsumableName(e.target.value)} />
                    <input className='create-input' type="text" placeholder="Enter consumable type" onChange={(e) => setConsumableType(e.target.value)} />
                    <input className='create-input' type="number" placeholder="Enter consumable weight" onChange={(e) => setConsumableWeight(e.target.value)} />
                    <input className='create-input' type="number" placeholder="Enter consumable dimensions" onChange={(e) => setConsumableDimensions(e.target.value)} />
                    <input className='create-input' type="number" placeholder="Enter consumable value" onChange={(e) => setConsumableValue(e.target.value)} />
                    <input className='create-input' type="text" placeholder="Enter consumable effect" onChange={(e) => setConsumableEffect(e.target.value)} />
                    <br/>
                    <button className='consumable-button' onClick={handleCreateConsumable}>Create a new Consumable</button>
                </div>
                
                <div className='create-consumable'>
                    <h1 className='i-n'>Mod Cons.</h1>
                    <input className='create-input' type="text" placeholder="Enter consumable name" onChange={(e) => setConsumableName(e.target.value)} />
                    <input className='create-input' type="text" placeholder="Enter consumable type" onChange={(e) => setConsumableType(e.target.value)} />
                    <input className='create-input' type="number" placeholder="Enter consumable weight" onChange={(e) => setConsumableWeight(e.target.value)} />
                    <input className='create-input' type="number" placeholder="Enter consumable dimensions" onChange={(e) => setConsumableDimensions(e.target.value)} />
                    <input className='create-input' type="number" placeholder="Enter consumable value" onChange={(e) => setConsumableValue(e.target.value)} />
                    <input className='create-input' type="text" placeholder="Enter consumable effect" onChange={(e) => setConsumableEffect(e.target.value)} />
                    <br/>
                    <button className='consumable-button' onClick={handleUpdateConsumable}>Modify consumable</button>
                </div>   
                </div>
            </div>
        </div>
    );
};

export default Item;