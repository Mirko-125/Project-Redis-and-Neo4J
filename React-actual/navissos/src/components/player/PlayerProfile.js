import { useParams, useState, useEffect } from "react";
import '../../styling/CrudContainer.css'
import './Player.css'


const PlayerProfile = () => {
    const [playerName, setPlayerName] = useState('');
    const [player, setPlayer] = useState(null)

    useEffect(() => {
        const currentUrl = window.location.href;
        console.log(currentUrl);
        const pathSegments = currentUrl.split('/');
        const name = pathSegments[pathSegments.length - 1];
        console.log(name)
        if (!player) {
        fetch(`http://localhost:5236/api/Player/GetOne?name=${name}`)
            .then(response => response.json())
            .then(data => {setPlayer(data); console.log(data)});
        }
    }, []);
    
    return (
        <div>
            {player && <div class="stats-bar">
                <div class="stat-item">
                    <strong>Name:</strong> {player.name}
                </div>
                <div class="stat-item">
                    <strong>Email:</strong> {player.email}
                </div>
                <div class="stat-item">
                    <strong>Level:</strong> {player.attributes.level}
                </div>
                <div class="stat-item">
                    <strong>Gold:</strong> {player.gold}
                </div>
                <div class="stat-item">
                    <strong>Honor:</strong> {player.honor}
                </div>
                <div class="stat-item">
                    <strong>Strength:</strong> {player.attributes.strength}
                </div>
                <div class="stat-item">
                    <strong>Agility:</strong> {player.attributes.agility}
                </div>
                <div class="stat-item">
                    <strong>Intelligence:</strong> {player.attributes.intelligence}
                </div>
                <div class="stat-item">
                    <strong>Stamina:</strong> {player.attributes.stamina}
                </div>
                <div class="stat-item">
                    <strong>Faith:</strong> {player.attributes.faith}
                </div>
                <div class="stat-item">
                    <strong>Experience:</strong> {player.attributes.experience}
                </div>
            </div>
            }
            <div>
                <h1 className='i-p'>Gear</h1>
                <div className='items'>
                    {player && player.equipment.equippedGear.map((g, idx) => (
                        <div className="player-item" key={g + idx}>
                            {g.name}<br/>
                            weight: {g.weight}<br/>
                            type: {g.type}<br/>
                            value: {g.value}<br/>
                            dimensions: {g.dimensions}<br/>
                            level: {g.attributes.level}<br/>
                            strength: {g.attributes.strength}<br/>
                            agility: {g.attributes.agility}<br/>
                            intelligence: {g.attributes.intelligence}<br/>
                            slot: {g.slot}<br/>
                            quality: {g.quality}<br/>
                        </div>
                    ))}
                </div>
            </div>
            <div>
                <h1 className='i-p'>Item</h1>
                <div className='items'>
                    {player && player.inventory.items.map((item, idx) => (
                        <div className="player-item" key={item+idx}> 
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
                        </div>
                    ))}
                </div>
                <div>
                    <h1 className='i-p'>Abilities</h1>
                    <div className='items'>
                        {player && player.abilities && player.abilities.map((a, idx) => (
                            <div className="player-item" key={a+idx}> 
                                {a.name}<br/>
                                damage: {a.damage}<br/>
                                cooldown: {a.cooldown}<br/>
                                range: {a.range}<br/>
                                special: {a.special}<br/>
                                heal: {a.heal}<br/>
                            </div>
                        ))}
                    </div>
                </div>
                <div>
                <h1 className='i-p'>Achievements</h1>
                    <div className='items'>
                        {player && player.achievements && player.achievements.map((a, idx) => (
                            <div className="player-item" key={a+idx}> 
                                {a.name}<br/>
                                type: {a.type}<br/>
                                points: {a.points}<br/>
                                conditions: {a.conditions}<br/>
                            </div>
                        ))}
                    </div>
                </div>
            </div>

        </div>
    )
}

export default PlayerProfile;