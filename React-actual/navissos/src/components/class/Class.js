import React, { useEffect, useState } from 'react';
import './Class.css';

const Class = () => {
    const [classId, setClassId] = useState('');
    const [className, setClassName] = useState('');
    const [classData, setClassData] = useState([]);
    const [name, setName] = useState('');
    const [baseStrength, setBaseStrength] = useState(0);
    const [baseAgility, setBaseAgility] = useState(0);
    const [baseIntelligence, setBaseIntelligence] = useState(0);
    const [baseStamina, setBaseStamina] = useState(0);
    const [baseFaith, setBaseFaith] = useState(0);
    const [baseExperience, setBaseExperience] = useState(0);
    const [baseLevel, setBaseLevel] = useState(0);
    const [levelGainStrength, setLevelGainStrength] = useState(0);
    const [levelGainAgility, setLevelGainAgility] = useState(0);
    const [levelGainIntelligence, setLevelGainIntelligence] = useState(0);
    const [levelGainStamina, setLevelGainStamina] = useState(0);
    const [levelGainFaith, setLevelGainFaith] = useState(0);
    const [levelGainExperience, setLevelGainExperience] = useState(0);
    const [levelGainLevel, setLevelGainLevel] = useState(0);

    const handleDeleteClass = () => {
        fetch(`http://localhost:5236/api/Class?className=${className}`, {
            method: 'DELETE'
        })
            .then(response => response.json())
            .then(data => {
                window.location.reload();
                console.log(data);
            })
            .catch(error => {

                console.error(error);
            });
    };


    const handleEditClass = () => {
        const classData = {
            id: classId,
            name: name
        };

        fetch('http://localhost:5236/api/Class/UpdateClass?classId=${id}&newName=${name}', {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(classData)
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

    const handleCreateClass = () => {
        const classData = {
            name: name,
            baseStrength: baseStrength,
            baseAgility: baseAgility,
            baseIntelligence: baseIntelligence,
            baseStamina: baseStamina,
            baseFaith: baseFaith,
            baseExperience: baseExperience,
            baseLevel: baseLevel,
            levelGainStrength: levelGainStrength,
            levelGainAgility: levelGainAgility,
            levelGainIntelligence: levelGainIntelligence,
            levelGainStamina: levelGainStamina,
            levelGainFaith: levelGainFaith,
            levelGainExperience: levelGainExperience,
            levelGainLevel: levelGainLevel
        };

        fetch('http://localhost:5236/api/Class', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(classData)
        })
            .then(response => response.json())
            .then(data => {
                window.location.reload();
                console.log(data);
            })
            .catch(error => {
                // Handle the error if needed
                console.error(error);
            });
    };

    useEffect(() => {
        fetch('http://localhost:5236/api/Class/GetAll')
            .then(response => response.json())
            .then(data => setClassData(data));
    }, []);

    return (
    <div className='class-wrap'>
        <div className='classFrame'>
            <div className='classes-data'>
                <h1 className='i-c'>All classes: </h1>
                {classData.map(classData => (
                    <div style={{
                        backgroundColor: '#ffdbac',
                        backgroundSize: 'cover',
                        width: '200px',
                        height: '100px',
                        border: 'none',
                        cursor: 'pointer',
                        margin: '3rem'
                    }}>{classData.id} - {classData.name}</div>
                ))}
            </div>
            <h1 className='i-c'>Admin: Create a new playable class</h1>
            <div className='create-class'>
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter name"
                    onChange={(e) => setName(e.target.value)}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Enter base strength"
                    onChange={(e) => setBaseStrength(parseInt(e.target.value))}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Enter base agility"
                    onChange={(e) => setBaseAgility(parseInt(e.target.value))}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Enter base intelligence"
                    onChange={(e) => setBaseIntelligence(parseInt(e.target.value))}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Enter base stamina"
                    onChange={(e) => setBaseStamina(parseInt(e.target.value))}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Enter base faith"
                    onChange={(e) => setBaseFaith(parseInt(e.target.value))}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Enter base experience"
                    onChange={(e) => setBaseExperience(parseInt(e.target.value))}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Enter base level"
                    onChange={(e) => setBaseLevel(parseInt(e.target.value))}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Enter level gain strength"
                    onChange={(e) => setLevelGainStrength(parseInt(e.target.value))}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Enter level gain agility"
                    onChange={(e) => setLevelGainAgility(parseInt(e.target.value))}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Enter level gain intelligence"
                    onChange={(e) => setLevelGainIntelligence(parseInt(e.target.value))}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Enter level gain stamina"
                    onChange={(e) => setLevelGainStamina(parseInt(e.target.value))}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Enter level gain faith"
                    onChange={(e) => setLevelGainFaith(parseInt(e.target.value))}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Enter level gain experience"
                    onChange={(e) => setLevelGainExperience(parseInt(e.target.value))}
                />
                <input
                    className='create-input'
                    type="number"
                    placeholder="Enter level gain level"
                    onChange={(e) => setLevelGainLevel(parseInt(e.target.value))}
                />
                <button onClick={handleCreateClass}>Create class</button>
            </div>
            <h1 className='i-c'>Admin: Edit a new playable class</h1>
            <div className='create-class'>
                <input
                    className='create-input'
                    type="number"
                    placeholder="Enter class id"
                    onChange={(e) => setClassId(e.target.value)}
                />
                <input
                    className='create-input'
                    type="text"
                    placeholder="Enter class' new name"
                    onChange={(e) => setName(parseInt(e.target.value))}
                />
                <button onClick={handleEditClass}>Edit class</button>
            </div>
            <h1 className='i-n'>Admin: Delete a class</h1>
            <div className='create-class'>
                <input type="text" placeholder="Enter class name" onChange={e => setClassName(e.target.value)} />
                <button onClick={handleDeleteClass}>Delete class</button>
            </div>
        </div>
    </div>
    );
}

export default Class;
