-- Can't fire a skill from the client, so we're receiving messages from the player on client side collision with our parrot volume.

function onFireEventServerSide( self, msg )	

	if ( msg.args == "Slow" ) then
        self:CastSkill{skillID = 795, optionalTargetID = msg.senderID}

    elseif ( msg.args == "Unslow" ) then
        self:CastSkill{skillID = 796, optionalTargetID = msg.senderID}
    end

end