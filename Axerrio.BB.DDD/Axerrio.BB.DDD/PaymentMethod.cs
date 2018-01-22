using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Axerrio.BB.DDD
{
    public class PaymentMethod
    {
        private string _alias;
        private string _cardNumber;
        private string _securityNumber;
        private string _cardHolderName;
        private DateTime _expiration;

        private int _cardTypeId;
        //public CardType CardType { get; private set; }

        int _Id;
        public virtual int Id
        {
            get
            {
                return _Id;
            }
            protected set
            {
                _Id = value;
            }
        }

        public string Alias => _alias;
        public string CardNumber => _cardNumber;
        public string SecurityNumber => _securityNumber;
        public string CardHolderName => _cardHolderName;
        public DateTime Expiration => _expiration;

        public int CardTypeId => _cardTypeId;

        protected PaymentMethod() { }

        public PaymentMethod(int cardTypeId, string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration)
        {

            _cardNumber = !string.IsNullOrWhiteSpace(cardNumber) ? cardNumber : throw new ArgumentException(nameof(cardNumber));
            _securityNumber = !string.IsNullOrWhiteSpace(securityNumber) ? securityNumber : throw new ArgumentException(nameof(securityNumber));
            _cardHolderName = !string.IsNullOrWhiteSpace(cardHolderName) ? cardHolderName : throw new ArgumentException(nameof(cardHolderName));

            if (expiration < DateTime.UtcNow)
            {
                throw new ArgumentException(nameof(expiration));
            }

            _alias = alias;
            _expiration = expiration;
            _cardTypeId = cardTypeId;
        }
    }
}
