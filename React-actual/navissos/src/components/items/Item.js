import React, { useEffect, useState } from 'react';
import './Item.css';

const Item = () => {
    const [item, setItem] = useState([]);
    const [itemType, setItemType] = useState('');
    const [gearId, setGearId] = useState('');
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
    const [consumableId, setConsumableId] = useState('');
    const [consumableName, setConsumableName] = useState('');
    const [consumableType, setConsumableType] = useState('');
    const [consumableWeight, setConsumableWeight] = useState(0);
    const [consumableDimensions, setConsumableDimensions] = useState(0);
    const [consumableValue, setConsumableValue] = useState(0);
    const [consumableEffect, setConsumableEffect] = useState('');

    const handleCreateGear = () => {
        const gearData = {
            name: gearName,
            type: gearType,
            weight: gearWeight,
            dimensions: gearDimensions,
            value: gearValue,
            slot: gearSlot,
            level: gearLevel,
            quality: gearQuality,
            strength: gearStrength,
            agility: gearAgility,
            intelligence: gearIntelligence,
            stamina: gearStamina,
            faith: gearFaith,
            experience: gearExperience
        };

        fetch('http://localhost:5236/api/NPC/AddNPC', {
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

    const handleCreateConsumable = () => {
        const consumableData = {
            name: consumableName,
            type: consumableType,
            weight: consumableWeight,
            dimensions: consumableDimensions,
            value: consumableValue,
            effect: consumableEffect
        };

        fetch('http://localhost:5236/api/Consumable/AddConsumable', {
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

    const handleItemType = () => {
        fetch(`http://localhost:5236/api/Item/GetItemByType?type=${itemType}`) 
            .then(response => response.json())
            .then(data => setItem(data));
    }

    useEffect(() => {
        fetch('http://localhost:5236/api/Item/GetAllItems') 
            .then(response => response.json())
            .then(data => setItem(data));
    }, []);
    
    const handleUpdateGear = () => {
        const gearData = {
            gearId: gearId,
            name: gearName,
            type: gearType,
            weight: gearWeight,
            dimensions: gearDimensions,
            value: gearValue,
            slot: gearSlot,
            level: gearLevel,
            quality: gearQuality,
            strength: gearStrength,
            agility: gearAgility,
            intelligence: gearIntelligence,
            stamina: gearStamina,
            faith: gearFaith,
            experience: gearExperience
        };

        fetch('http://localhost:5236/api/Gear/UpdateGear', {
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
            consumableId: consumableId,
            name: consumableName,
            type: consumableType,
            weight: consumableWeight,
            dimensions: consumableDimensions,
            value: consumableValue,
            effect: consumableEffect
        };

        fetch('http://localhost:5236/api/Gear/UpdateGear', {
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
                {item.map(item => (
                <button key={item.id} style={{
                    backgroundSize: 'cover',
                    width: '200px',
                    height: '100px', 
                    border: 'none',
                    cursor: 'pointer',
                    margin: '3rem'
                }}>
                    {item.name}
                </button>
            ))}
            </div>
            <div className='item-type'>
                <h1 className='i-a'>Find your item by type</h1>
                <input type="text" placeholder="Enter item type" onChange={e => setItemType(e.target.value)} />
                <button onClick={handleItemType}>Find it</button>
                <div className='found'>
                    {itemType && item.map(item => (
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
                <div className='item-type'>
                <h1 className='i-a'>Find your item by type</h1>
                <input type="text" placeholder="Enter item name" onChange={e => setItemType(e.target.value)} />
                <button onClick={handleItemType}>Find it</button>
                <div className='found'>
                    {itemType && item.map(item => (
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
            </div>
            <div className='split'>
                <div className='gear'>
                    <h1 className='i-n'>+ Gear</h1>
                        <div className='create-gear'>
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
                            <h1 className='i-n'>Modify Gear</h1>
                            <div className='create-gear'>
                                <input className='create-input' type="number" placeholder="Enter gear ID" onChange={e => setGearId(e.target.value)} />
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
                <div className='consumable'>
                <h1 className='i-n'>+ Cons.</h1>
                <div className='create-consumable'>
                    <input className='create-input' type="text" placeholder="Enter consumable name" onChange={(e) => setConsumableName(e.target.value)} />
                    <input className='create-input' type="text" placeholder="Enter consumable type" onChange={(e) => setConsumableType(e.target.value)} />
                    <input className='create-input' type="number" placeholder="Enter consumable weight" onChange={(e) => setConsumableWeight(e.target.value)} />
                    <input className='create-input' type="number" placeholder="Enter consumable dimensions" onChange={(e) => setConsumableDimensions(e.target.value)} />
                    <input className='create-input' type="number" placeholder="Enter consumable value" onChange={(e) => setConsumableValue(e.target.value)} />
                    <input className='create-input' type="text" placeholder="Enter consumable effect" onChange={(e) => setConsumableEffect(e.target.value)} />
                    <br/>
                    <button className='consumable-button' onClick={handleCreateConsumable}>Create a new Consumable</button>
                </div>
                <h1 className='i-n'>Mod Cons.</h1>
                <div className='create-consumable'>
                    <input className='create-input' type="number" placeholder="Enter consumable ID" onChange={e => setConsumableId(e.target.value)} />
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