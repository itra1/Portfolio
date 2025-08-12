using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface INetOutPacket
{
    void Write(NetOutputStream stream);
}

