-- constructor
Vector = {}
Vector.__index = Vector


function Vector.new(x,y,z)
	local newobj
	if type(x) == 'table' then
		newobj = x
	else
		newobj = { x = x, y = y, z = z }
	end
	setmetatable(newobj, Vector)
	return(newobj)
end

function Vector.__add(a,b)
	return Vector.new(a.x + b.x, a.y + b.y, a.z + b.z)
end

function Vector.__sub(a,b)
	return Vector.new(a.x - b.x, a.y - b.y, a.z - b.z)
end

function Vector.__unm(a)
	return Vector.new(-a.x, -a.y, -a.z)
end

function Vector.__mul(a,b)
	if type(a) == 'table' then
		if type(b) == 'table' then
			-- dot product
			return a.x * b.x + a.y * b.y + a.z * b.z;
		else
			-- scalar product
			return Vector.new(a.x * b, a.y * b, a.z * b)
		end
	else
		-- scalar product
		return Vector.new(a * b.x, a * b.y, a * b.z)
	end
end

function Vector.__div(self, a)
    if type(a) == 'number' then
        return Vector.new(self.x / a, self.y / a, self.z / a)
    end
end

function Vector.length(self)
	return math.sqrt(self:sqrLength())
end

function Vector.sqrLength(self)
    return self.x * self.x + self.y * self.y + self.z * self.z
end

function Vector.normalize(self)
    return  self / self:length()
end

function Vector.__eq(a, b)
    return (a.x == b.x and a.y == b.y and a.z == b.z)
end

function Vector.__newindex(self, key, value)
    print("ERROR - do not assign values to a vector other than x, y, or z")
end

function Vector.print(self)
    print(self.x, self.y, self.z)
end
