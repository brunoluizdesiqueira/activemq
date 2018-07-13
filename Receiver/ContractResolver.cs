namespace Receiver
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Amqp;
    using Amqp.Types;
    using Amqp.Serialization;

    public class ContractResolver : IContractResolver
    {
        public IList<string> PrefixList { get; set; }

        AmqpContract IContractResolver.Resolve(Type type)
        {
            if (PrefixList != null)
            {
                bool matched = false;
                for (int i = 0; i < this.PrefixList.Count; i++)
                {
                    if (type.FullName.StartsWith(this.PrefixList[i], StringComparison.Ordinal))
                    {
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                {
                    return null;
                }
            }

            AmqpContract baseContract = null;
            if (type.BaseType != typeof(object))
            {
                baseContract = ((IContractResolver)this).Resolve(type.BaseType);
            }

            int order = 0;
            var memberList = new List<AmqpMember>();
            if (baseContract != null)
            {
                for (int i = 0; i < baseContract.Members.Length; i++)
                {
                    memberList.Add(baseContract.Members[i]);
                    order = Math.Max(order, baseContract.Members[i].Attribute.Order);
                }
            }

            order++;
            MemberInfo[] memberInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (MemberInfo memberInfo in memberInfos)
            {
                if (memberInfo.DeclaringType != type)
                {
                    continue;
                }

                if (memberInfo is PropertyInfo)
                {
                    memberList.Add(new AmqpMember()
                    {
                        Attribute = new AmqpMemberAttribute()
                        {
                            Name = memberInfo.Name,
                            Order = order++
                        },
                        Info = memberInfo,
                    });
                }
            }

            List<Type> knownTypes = new List<Type>();
            foreach (Type t in type.Assembly.GetTypes())
            {
                if (t.BaseType == type)
                {
                    knownTypes.Add(t);
                }
            }

            var contract = new AmqpContract(type)
            {
                Attribute = new AmqpContractAttribute()
                {
                    Name = type.FullName,
                    Encoding = EncodingType.List
                },
                Members = memberList.ToArray(),
                Provides = knownTypes.ToArray(),
                BaseContract = baseContract
            };

            return contract;
        }
    }
}
