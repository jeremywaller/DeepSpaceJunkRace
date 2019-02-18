using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ES2UserType_EnemyKillStat : ES2Type
{
	public override void Write(object obj, ES2Writer writer)
	{
		EnemyKillStat data = (EnemyKillStat)obj;
		// Add your writer.Write calls here.
		writer.Write(data.ScoreName);
		writer.Write(data.KillsThisSession);
		writer.Write(data.KillsLifetime);
		writer.Write(data.KillsLastSession);

	}
	
	public override object Read(ES2Reader reader)
	{
		EnemyKillStat data = new EnemyKillStat();
		Read(reader, data);
		return data;
	}

	public override void Read(ES2Reader reader, object c)
	{
		EnemyKillStat data = (EnemyKillStat)c;
		// Add your reader.Read calls here to read the data into the object.
		data.ScoreName = reader.Read<System.String>();
		data.KillsThisSession = reader.Read<System.Int32>();
		data.KillsLifetime = reader.Read<System.Int32>();
		data.KillsLastSession = reader.Read<System.Int32>();

	}
	
	/* ! Don't modify anything below this line ! */
	public ES2UserType_EnemyKillStat():base(typeof(EnemyKillStat)){}
}